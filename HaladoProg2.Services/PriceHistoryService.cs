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
	public interface IPriceHistoryService
	{
		Task<List<PriceHistory>> GetAllAsync();
		Task<PriceHistory> GetAsync(int historyId);
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
			return _dbContext.PriceHistories.AsEnumerable().ToList();
		}

		public async Task<PriceHistory> GetAsync(int historyId)
		{
			var history = await _dbContext.PriceHistories.Where(p => p.Id == historyId).FirstAsync();
			return history;
		}
	}
}
