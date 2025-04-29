using HaladoProg2.DataContext.Dtos.Trade;
using HaladoProg2.Services;
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
			var user = await _userService.GetAsync(tradeBuyDto.UserId);
			if (user == null)
				return NotFound("Nincs ilyen felhasználó!");

			if (user.UserMoney < tradeBuyDto.PriceInHuf)
				return BadRequest("Nincs elegendő pénzed a megvásárláshoz!");

			var targetWallet = await _walletService.GetUserWalletWithCryptoAsync(tradeBuyDto.UserId, tradeBuyDto.CryptoId);
			if (targetWallet == null)
				return NotFound("A felhasználónak hiányzik az ilyen típusú kriptó tárcája");
			
			var targetCrypto = targetWallet.Crypto;

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
			result &= await _userService.ModifyMoneyAsync(
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
			var user = await _userService.GetAsync(tradeSellDto.UserId);
			if (user == null)
				return NotFound("Nincs ilyen felhasználó!");

			var targetWallet = await _walletService.GetUserWalletWithCryptoAsync(tradeSellDto.UserId, tradeSellDto.CryptoId);
			if (targetWallet == null)
				return NotFound("A felhasználónak hiányzik az ilyen típusú kriptó tárcája");

			if (targetWallet.CryptoCount < tradeSellDto.Quantity)
				return BadRequest("Nincs elegendő mennyiségű kriptód az eladáshoz!");

			var targetCrypto = targetWallet.Crypto;

			var earnedHuf = tradeSellDto.Quantity * targetCrypto.CurrentPrice;

			var result = await _walletService.UpdateAsync(
				targetWallet.Id,
				targetWallet.UserId,
				targetWallet.CryptoId,
				targetWallet.CryptoCount - tradeSellDto.Quantity); // we give negative ammount of the sold crypto
			if (!result)
				return BadRequest("Hiba történt a tárca frissítése közben!");

			// change overall available crypto count
			result &= await _cryptoService.UpdateAsync(
				targetWallet.CryptoId,
				targetCrypto.Name,
				targetCrypto.AvailableQuantity + tradeSellDto.Quantity, // change the quantity
				targetCrypto.CurrentPrice);
			if (!result)
				return BadRequest("Error while updating the crypto available quantity!");

			// create a transaction
			result &= await _transactionService.CreateAsync(
				targetWallet.UserId,
				targetWallet.CryptoId,
				tradeSellDto.Quantity,
				targetCrypto.CurrentPrice,
				DateTime.Now,
				true);
			if (!result)
				return BadRequest("Error while creating a new transaction!");

			// make the user spend the money
			result &= await _userService.ModifyMoneyAsync(
				user.Id,
				earnedHuf); // modify user money
			if (!result)
				return BadRequest("Error while modifying user money!");

			return Ok(new TradeSellResultDto
			{
				WalletId = targetWallet.Id,
				RecievedMoney = earnedHuf
			});
		}
	}
}
