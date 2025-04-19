using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HaladoProg2.DataContext.Context;
using HaladoProg2.DataContext.Entities;

using Microsoft.EntityFrameworkCore;

namespace HaladoProg2.Services
{
	public interface ITransactionService
	{
		Task<bool> CreateAsync(int userId, int cryptoId, double transactionQuantity, double transactionPrice);
		Task<Transaction?> GetAsync(int transactionId);
	}

	public class TransactionService : ITransactionService
	{
		private readonly AppDbContext _dbContext;
		public TransactionService(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<bool> CreateAsync(int userId, int cryptoId, double transactionQuantity, double transactionPrice)
		{
			if (transactionQuantity < 0 || transactionPrice < 0)
				return false; // cant be negative

			_dbContext.Transactions.Add(new Transaction
			{
				UserId = userId,
				CryptoId = cryptoId,
				TransactionPrice = transactionPrice,
				TransactionQuantity = transactionQuantity
			});
			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<Transaction?> GetAsync(int transactionId)
		{
			var transaction = await _dbContext.Transactions.Where(t => t.Id == transactionId).FirstOrDefaultAsync();
			return transaction;
		}
	}
}
