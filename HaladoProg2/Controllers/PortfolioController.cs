using HaladoProg2.DataContext.Dtos.Portfolio;
using HaladoProg2.DataContext.Entities;
using HaladoProg2.Services;
using Microsoft.AspNetCore.Mvc;

namespace HaladoProg2.Controllers
{
	[ApiController]
	[Route("api/portfolio")]
	public class PortfolioController : ControllerBase
	{
		private readonly ICryptoService _cryptoService;
		private readonly IUserService _userService;

		public PortfolioController(IUserService userService, ICryptoService cryptoService)
		{
			_userService = userService;
			_cryptoService = cryptoService;
		}

		[HttpGet("{userId}")]
		public async Task<IActionResult> GetPortfolioAsync(int userId)
		{
			var user = await _userService.GetIncludesAsync(userId);
			if (user == null)
				return NotFound("Nincs ilyen felhasználó!");

			var userWallets = user.Wallets;
			var userBuyTransactions = user.Transactions.Where(t => t.IsSelling == false).ToList();
			var userSellTransactions = user.Transactions.Where(t => t.IsSelling).ToList();
			
			var walletCount = userWallets.Count;
			var buyTransCount = userBuyTransactions.Count;
			var sellTransCount = userSellTransactions.Count;
			var totalWorth = userWallets.Select(SumCryptoFromWalletAsync).Sum(result => result.Result);
			var portfolioDto = new PortfolioDto
			{
				UserName = user.Username,
				WalletsCount = walletCount,
				BuyTransactionsCount = buyTransCount,
				SellTransactionsCount = sellTransCount,
				TotalNetWorth = totalWorth,
				PortfolioWallets = walletCount <= 0 ? [] :
								userWallets.Where(w => w.CryptoCount > 0).ToList().ConvertAll(w => ConvertWalletToDtoAsync(w).Result),
				PortfolioTransactionsBuy = buyTransCount <= 0 ? [] : userBuyTransactions.ConvertAll(ConvertTransactionToDto),
				PortfolioTransactionsSell = sellTransCount <= 0 ? [] : userSellTransactions.ConvertAll(ConvertTransactionToDto)
			};
			return Ok(portfolioDto);
		}

		private async Task<double> SumCryptoFromWalletAsync(Wallet w)
		{
			var crypto = await _cryptoService.GetAsync(w.CryptoId);
			if(crypto == null)
				return 0;
			return w.CryptoCount * crypto.CurrentPrice;
		}

		private async Task<PortfolioWalletDto> ConvertWalletToDtoAsync(Wallet w)
		{
			var crypto = await _cryptoService.GetAsync(w.CryptoId);
			return new PortfolioWalletDto()
			{
				WalletId = w.Id,
				WalletValue = crypto == null ? 0 : crypto.CurrentPrice * w.CryptoCount,
				CryptoId = w.CryptoId,
				CryptoCount = w.CryptoCount,
				CryptoName = crypto == null ? "NULL" : crypto.Name,
			};
		}
		
		private PortfolioTransactionsDto ConvertTransactionToDto(Transaction t)
		{
			return new PortfolioTransactionsDto()
			{
				TransactionId = t.Id,
				TransactionDate = t.TransactionTime,
				CryptoCount = t.TransactionQuantity,
				CryptoId = t.CryptoId,
				OverallValue = t.TransactionPrice
			};
		}
	}
}
