using HaladoProg2.DataContext.Dtos.Trade;
using HaladoProg2.Services;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaladoProg2.Controllers
{
	[ApiController]
	[Route("api/trade")]
	public class TradeController : ControllerBase
	{
		private readonly ITransactionService _transactionService;
		private readonly IWalletService _walletService;
		private readonly ICryptoService _cryptoService;
		private readonly IUserService _userService;

		public TradeController(ITransactionService transactionService, IUserService userService, IWalletService walletService, ICryptoService cryptoService)
		{
			_transactionService = transactionService;
			_userService = userService;
			_walletService = walletService;
			_cryptoService = cryptoService;
		}

		[HttpPost("buy")]
		public async Task<IActionResult> BuyCryptoAsync([FromBody] TradeBuyDto tradeBuyDto)
		{
			var user = await _userService.GetIncludesAsync(tradeBuyDto.UserId);
			if (user == null)
				return NotFound("Nincs ilyen felhasználó!");

			if (user.UserMoney < tradeBuyDto.PriceInHuf)
				return BadRequest("Nincs elegendő pénzed a megvásárláshoz!");

			if (user.Wallets == null)
				user.Wallets = new List<DataContext.Entities.Wallet>();

			var targetWallet = user.Wallets.AsQueryable().Include(w => w.Crypto).FirstOrDefault(w => w.CryptoId == tradeBuyDto.CryptoId);
			if(targetWallet == null) // the user doesnt have a wallet, that can store the given crypto
			{
				// create a new wallet for the user to store this crypto
				var result1 = await _walletService.CreateAsync(user.Id, tradeBuyDto.CryptoId, 0);
				if (!result1)
					return BadRequest("Hiba történt amikor a felhasználóhoz új tárcát akartunk hozzáadni!");
				targetWallet = user.Wallets.AsQueryable().Include(w => w.Crypto).FirstOrDefault(w => w.CryptoId == tradeBuyDto.CryptoId);
			}

			var targetCrypto = targetWallet.Crypto;
			if (targetCrypto == null)
				return BadRequest("Nincs linkelve a kriptóId-hez objektum (backend hiba)!");

			var cryptoQuantity = tradeBuyDto.PriceInHuf / targetCrypto.CurrentPrice;

			if (targetCrypto.AvailableQuantity < cryptoQuantity)
				return BadRequest("Nincs ennyi a megvásárolni kívánt kriptóból a piacon!");

			// change in wallet
			var result = await _walletService.UpdateAsync(
				targetWallet.Id,
				targetWallet.UserId,
				targetWallet.CryptoId,
				targetWallet.CryptoCount + cryptoQuantity); // add the bought crypto
			if (!result)
				return BadRequest("Error while updating the wallet!");

			// change overall available crypto count
			result &= await _cryptoService.UpdateAsync(
				targetWallet.CryptoId,
				targetCrypto.Name,
				targetCrypto.AvailableQuantity - cryptoQuantity, // change the quantity
				targetCrypto.CurrentPrice);
			if (!result)
				return BadRequest("Error while updating the crypto available quantity!");

			// create a transaction
			result &= await _transactionService.CreateAsync(
				targetWallet.UserId,
				targetWallet.CryptoId,
				cryptoQuantity,
				targetCrypto.CurrentPrice,
				DateTime.Now,
				false);
			if (!result)
				return BadRequest("Error while creating a new transaction!");

			// make the user spend the money
			result &= await _userService.ModifyMoney(
				user.Id,
				-tradeBuyDto.PriceInHuf); // modify user money
			if (!result)
				return BadRequest("Error while modifying user money!");

			return Ok(new TradeBuyResultDto
			{
				WalletId = targetWallet.Id,
				BoughtQuantity = cryptoQuantity
			});
		}

		[HttpPost("sell")]
		public async Task<IActionResult> SellCryptoAsync([FromBody] TradeSellDto tradeSellDto)
		{
			var user = await _userService.GetIncludesAsync(tradeSellDto.UserId);
			if (user == null)
				return NotFound("Nincs ilyen felhasználó!");

			var wallet = user.Wallets.AsQueryable().Include(w => w.Crypto).FirstOrDefault(w => w.CryptoId == tradeSellDto.CryptoId);
			if (wallet == null)
				return NotFound("Nincs olyan pénztárcád, amiből eltudnád adni az adott kriptó típust!");

			if (wallet.CryptoCount < tradeSellDto.Quantity)
				return BadRequest("Nincs elegendő mennyiségű kriptód az eladáshoz!");

			var targetCrypto = wallet.Crypto;
			if (targetCrypto == null)
				return BadRequest("Nincs linkelve a kriptóId-hez objektum (backend hiba)!");

			var earnedHuf = tradeSellDto.Quantity * targetCrypto.CurrentPrice;

			var result = await _walletService.UpdateAsync(
				wallet.Id,
				wallet.UserId,
				wallet.CryptoId,
				wallet.CryptoCount - tradeSellDto.Quantity); // we give negative ammount of the sold crypto
			if (!result)
				return BadRequest("Hiba történt a tárca frissítése közben!");

			// change overall available crypto count
			result &= await _cryptoService.UpdateAsync(
				wallet.CryptoId,
				targetCrypto.Name,
				targetCrypto.AvailableQuantity + tradeSellDto.Quantity, // change the quantity
				targetCrypto.CurrentPrice);
			if (!result)
				return BadRequest("Error while updating the crypto available quantity!");

			// create a transaction
			result &= await _transactionService.CreateAsync(
				wallet.UserId,
				wallet.CryptoId,
				tradeSellDto.Quantity,
				targetCrypto.CurrentPrice,
				DateTime.Now,
				true);
			if (!result)
				return BadRequest("Error while creating a new transaction!");

			// make the user spend the money
			result &= await _userService.ModifyMoney(
				user.Id,
				earnedHuf); // modify user money
			if (!result)
				return BadRequest("Error while modifying user money!");

			return Ok(new TradeSellResultDto
			{
				WalletId = wallet.Id,
				RecievedMoney = earnedHuf
			});
		}
	}
}
