using HaladoProg2.DataContext.Dtos.Transaction;
using HaladoProg2.Services;

using Microsoft.AspNetCore.Mvc;

namespace HaladoProg2.Controllers
{
	[ApiController]
	[Route("api/transactions")]
	public class TransactionController : ControllerBase
	{
		private readonly ITransactionService _transactionService;
		private readonly IUserService _userService;

		public TransactionController(ITransactionService transactionService, IUserService userService)
		{
			_transactionService = transactionService;
			_userService = userService;
		}

		[HttpGet("{userId}")]
		public async Task<IActionResult> GetTransactionsAsync(int userId)
		{
			var user = await _userService.GetAsync(userId);
			if (user == null)
				return NotFound("Nincs ilyen felhasználó!");
			
			var transactions = await _transactionService.GetUserAllAsync(userId);
			if (transactions.Count <= 0)
				return NotFound("Ennek a felhasználónak nincs tranzakciós története!");

			var result = transactions.OrderBy(t => t.TransactionTime).ToList().ConvertAll(c => new TransactionDataDto
			{
				Id = c.Id,
				CryptoId = c.CryptoId,
				TransactionTime = c.TransactionTime,
				Selling = c.IsSelling
			});
			return Ok(result);
		}
		[HttpGet("details/{transactionId}")]
		public async Task<IActionResult> GetTransactionDetailsAsync(int transactionId)
		{
			var transaction = await _transactionService.GetAsync(transactionId);
			if (transaction == null)
				return NotFound("Nincs ilyen Id-vel ellátott tranzakció!");

			var result = new TransactionDataDto
			{
				Id = transactionId,
				CryptoId = transaction.CryptoId,
				TransactionPrice = transaction.TransactionPrice,
				TransactionQuantity = transaction.TransactionQuantity,
				TransactionSpentMoney = transaction.TransactionPrice * transaction.TransactionQuantity,
				TransactionTime = transaction.TransactionTime,
				Selling = transaction.IsSelling,
			};
			return Ok(result);
		}
	}
}
