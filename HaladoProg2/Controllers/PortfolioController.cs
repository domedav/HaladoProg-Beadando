using HaladoProg2.DataContext.Dtos.Trade;
using HaladoProg2.Services;

using Microsoft.AspNetCore.Mvc;

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

		}
	}
}
