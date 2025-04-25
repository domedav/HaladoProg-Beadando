using HaladoProg2.DataContext.Dtos.Portfolio;
using HaladoProg2.DataContext.Entities;
using HaladoProg2.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaladoProg2.Controllers
{
	[ApiController]
	[Route("api/portfolio")]
	public class PortfolioController : ControllerBase
	{
		private readonly ITransactionService _transactionService;
		private readonly IWalletService _walletService;
		private readonly ICryptoService _cryptoService;
		private readonly IUserService _userService;

		public PortfolioController(ITransactionService transactionService, IUserService userService, IWalletService walletService, ICryptoService cryptoService)
		{
			_transactionService = transactionService;
			_userService = userService;
			_walletService = walletService;
			_cryptoService = cryptoService;
		}

		[HttpGet("{userId}")]
		public async Task<IActionResult> GetPortfolio(int userId)
		{
			var user = await _userService.GetIncludesAsync(userId);
			if (user == null)
				return NotFound("Nincs ilyen felhasználó!");

			var userWallets = user.Wallets;
			var userBuyTransactions = user.Transactions.Where(t => t.IsSelling == false).ToList();
			var userSellTransactions = user.Transactions.Where(t => t.IsSelling == true).ToList();
			
			var walletCount = userWallets.Count;
			var buyTransCount = userBuyTransactions.Count();
			var sellTransCount = userSellTransactions.Count();
			var totalWorth = userWallets.Select(SumCryptoFromWalletAsync).Sum(result => result.Result);
			var portfolioDto = new PortfolioDto
			{
				UserName = user.Username,
				WalletsCount = walletCount,
				BuyTransactionsCount = buyTransCount,
				SellTransactionsCount = sellTransCount,
				TotalNetWorth = totalWorth,
				PortfolioWallets = walletCount <= 0 ? new List<PortfolioWalletDto>() : userWallets.ConvertAll(w => ConvertWalletToDto(w).Result),
				PortfolioTransactionsBuy = buyTransCount <= 0 ? new List<PortfolioTransactionsDto>() : userBuyTransactions.ConvertAll(ConvertTransactionToDto),
				PortfolioTransactionsSell = sellTransCount <= 0 ? new List<PortfolioTransactionsDto>() : userSellTransactions.ConvertAll(ConvertTransactionToDto)
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

		private async Task<PortfolioWalletDto> ConvertWalletToDto(Wallet w)
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
