using HaladoProg2.DataContext.Context;
using HaladoProg2.DataContext.Entities;

using Microsoft.EntityFrameworkCore;

namespace HaladoProg2.Services
{
	public interface IPriceHistoryService
	{
		Task<List<PriceHistory>> GetAllAsync();
		Task<PriceHistory?> GetAsync(int historyId);
		Task<bool> CreateAsync(int cryptoId, double currentPrice, DateTime time);
	}

	public class PriceHistoryService : IPriceHistoryService
	{
		private readonly AppDbContext _dbContext;
		public PriceHistoryService(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<bool> CreateAsync(int cryptoId, double currentPrice, DateTime time)
		{
			if (currentPrice < 0)
				return false; // cant be negative

			_dbContext.PriceHistories.Add(new PriceHistory
			{
				CryptoId = cryptoId,
				CurrentPrice = currentPrice,
				Time = time
			});
			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<List<PriceHistory>> GetAllAsync()
		{
			return await _dbContext.PriceHistories.AsQueryable().ToListAsync();
		}

		public async Task<PriceHistory?> GetAsync(int historyId)
		{
			var history = await _dbContext.PriceHistories.FirstOrDefaultAsync(p => p.Id == historyId);
			return history;
		}
	}
}
