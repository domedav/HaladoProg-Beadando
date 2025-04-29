using HaladoProg2.DataContext.Dtos.Wallet;
using HaladoProg2.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace HaladoProg2.Controllers
{
	[ApiController]
	[Route("api/wallet")]
	public class WalletController : ControllerBase
	{
		private readonly IWalletService _walletService;
		private readonly IUserService _userService;

		public WalletController(IWalletService walletService, IUserService userService)
		{
			_walletService = walletService;
			_userService = userService;
		}

		[HttpGet("{userId}")]
		public async Task<IActionResult> GetWalletAsync(int userId)
		{
			var user = await _userService.GetAsync(userId);
			if (user == null)
				return NotFound("A kért felhasználó nem létezik!");

			if (user.Wallets.Count <= 0)
				return NotFound("A felhasználónak nincs létező kriptó tárcája!");

			var result = new WalletGetDto
			{
				TotalMoneyHuf = await _userService.GetAllWalletWorthAsync(userId),
				TotalCryptoTypesCount = user.Wallets.Count,
				Wallets = (await _walletService.GetUserWallets(userId)).ConvertAll(c => new WalletGetCryptoDto
				{
					Id = c.Id,
					CryptoId = c.CryptoId,
					CryptoCount = c.CryptoCount,
					Value = c.Crypto.CurrentPrice * c.CryptoCount,
				})
			};
			return Ok(result);
		}

		[HttpPut("{userId}")]
		public async Task<IActionResult> UpdateWalletAsync(int userId, [FromBody] WalletUpdateDto walletUpdateDto)
		{
			var user = await _userService.GetAsync(userId);
			if (user == null)
				return NotFound("A kért felhasználó nem létezik!");

			var wallet = await _walletService.GetAsync(walletUpdateDto.WalletId);

			if (wallet == null)
				return NotFound("A kért tárca nem létezik!");

			var result = await _walletService.UpdateAsync(walletUpdateDto.WalletId, userId, wallet.CryptoId, walletUpdateDto.NewAmount);
			return Ok(result);
		}

		[HttpDelete("{userId}")]

		public async Task<IActionResult> DeleteWalletAsync(int userId, [FromBody] WalletDeleteDto walletDeleteDto)
		{
			var user = await _userService.GetAsync(userId);
			if (user == null)
				return NotFound("A kért felhasználó nem létezik!");

			var wallet = await _walletService.GetAsync(walletDeleteDto.WalletId);
			if (wallet == null)
				return NotFound("A kért tárca nem létezik!");
			
			var isOwnedWallet = wallet.UserId == userId;
			if (!isOwnedWallet)
				return BadRequest("A kért pénztárca nem a te tulajdonod!");

			var result = await _walletService.DeleteAsync(walletDeleteDto.WalletId);
			if(!result)
				return BadRequest("A kért tárca Id nem található!");
			return Ok(result);
		}
	}
}
