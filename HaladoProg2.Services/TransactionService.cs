using HaladoProg2.DataContext.Context;
using HaladoProg2.DataContext.Entities;

using Microsoft.EntityFrameworkCore;

namespace HaladoProg2.Services
{
	public interface ITransactionService
	{
		Task<bool> CreateAsync(int userId, int cryptoId, double transactionQuantity, double transactionPrice, DateTime transactionTime, bool selling);
		Task<Transaction?> GetAsync(int transactionId);
		Task<List<Transaction>> GetUserBuyingAsync(int userId);
		Task<List<Transaction>> GetUserAllAsync(int userId);
		Task<double> GetSumUserBuyingAsync(int userId);
		Task<double> GetSumUserBuyingCryptoAsync(int userId, int cryptoId);
		Task<List<Transaction>> GetUserSellingAsync(int userId);
		Task<double> GetSumUserSellingAsync(int userId);
		Task<double> GetSumUserSellingCryptoAsync(int userId, int cryptoId);

	}

	public class TransactionService : ITransactionService
	{
		private readonly AppDbContext _dbContext;
		public TransactionService(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<bool> CreateAsync(int userId, int cryptoId, double transactionQuantity, double transactionPrice, DateTime transactionTime, bool selling)
		{
			if (transactionQuantity < 0 || transactionPrice < 0)
				return false; // cant be negative

			_dbContext.Transactions.Add(new Transaction
			{
				UserId = userId,
				CryptoId = cryptoId,
				TransactionPrice = transactionPrice,
				TransactionQuantity = transactionQuantity,
				TransactionTime = transactionTime,
				IsSelling = selling
			});
			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<Transaction?> GetAsync(int transactionId)
		{
			var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == transactionId);
			return transaction;
		}

		public async Task<List<Transaction>> GetUserBuyingAsync(int userId)
		{
			var result = await _dbContext.Transactions.Where(t => !t.IsSelling && t.UserId == userId).ToListAsync();
			return result;
		}

		public async Task<List<Transaction>> GetUserAllAsync(int userId)
		{
			var result = await _dbContext.Transactions.Where(t => t.UserId == userId).ToListAsync();
			return result;
		}

		public async Task<double> GetSumUserBuyingAsync(int userId)
		{
			var buying = await GetUserBuyingAsync(userId);
			return buying.Sum(item => item.TransactionQuantity * item.TransactionPrice);
		}

		public async Task<double> GetSumUserBuyingCryptoAsync(int userId, int cryptoId)
		{
			var result = await _dbContext.Transactions.Where(t => !t.IsSelling && t.UserId == userId && t.CryptoId == cryptoId).ToListAsync();
			return result.Sum(item => item.TransactionQuantity * item.TransactionPrice);
		}

		public async Task<List<Transaction>> GetUserSellingAsync(int userId)
		{
			var result = await _dbContext.Transactions.Where(t => t.IsSelling && t.UserId == userId).ToListAsync();
			return result;
		}

		public async Task<double> GetSumUserSellingAsync(int userId)
		{
			var buying = await GetUserSellingAsync(userId);
			return buying.Sum(item => item.TransactionQuantity * item.TransactionPrice);
		}

		public async Task<double> GetSumUserSellingCryptoAsync(int userId, int cryptoId)
		{
			var result = await _dbContext.Transactions.Where(t => t.IsSelling && t.UserId == userId && t.CryptoId == cryptoId).ToListAsync();
			return result.Sum(item => item.TransactionQuantity * item.TransactionPrice);
		}
	}
}
