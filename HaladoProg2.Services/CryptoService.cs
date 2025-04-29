using HaladoProg2.DataContext.Context;
using HaladoProg2.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace HaladoProg2.Services
{
	public interface ICryptoService
	{
		Task<List<Crypto>> GetAllAsync();
		Task<Crypto?> GetAsync(int cryptoId);
		Task<int?> GetCryptoIdByNameAsync(string name);
		Task<List<PriceHistory>> GetPriceHistoriesAsync(int cryptoId);
		Task<bool> CreateAsync(string name, double availableQuantity, double currentPrice);
		Task<bool> UpdateAsync(int cryptoId, string name, double availableQuantity, double currentPrice);
		Task<bool> DeleteAsync(int cryptoId);
	}

	public class CryptoService : ICryptoService
	{
		private readonly AppDbContext _dbContext;
		public CryptoService(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<PriceHistory>> GetPriceHistoriesAsync(int cryptoId)
		{
			var crypto = await _dbContext.CryptoCurrencies.Include(c => c.PriceHistories).FirstOrDefaultAsync(c => c.Id == cryptoId);
			return crypto == null ? [] : crypto.PriceHistories;
		}

		public async Task<bool> CreateAsync(string name, double availableQuantity, double currentPrice)
		{
			if (name.Trim() == string.Empty ||
				availableQuantity < 0 ||
				currentPrice < 0) // invalid data
				return false;

			_dbContext.CryptoCurrencies.Add(new Crypto
			{
				Name = name,
				AvailableQuantity = availableQuantity,
				CurrentPrice = currentPrice,
				ContainingWallets = [],
				Transactions = [],
				PriceHistories = [],
			});

			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteAsync(int cryptoId)
		{
			if (!_dbContext.CryptoCurrencies.Any(c => c.Id == cryptoId))
				return false; // no such crypto

			_dbContext.CryptoCurrencies.Remove(await _dbContext.CryptoCurrencies.FirstAsync(c => c.Id == cryptoId));

			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<List<Crypto>> GetAllAsync()
		{
			return await _dbContext.CryptoCurrencies.AsQueryable().ToListAsync();
		}

		public async Task<Crypto?> GetAsync(int cryptoId)
		{
			var crypto = await _dbContext.CryptoCurrencies.FirstOrDefaultAsync(c => c.Id == cryptoId);
			return crypto;
		}

		public async Task<int?> GetCryptoIdByNameAsync(string name)
		{
			var crypto = await _dbContext.CryptoCurrencies.FirstOrDefaultAsync(c => c.Name == name);
			return crypto?.Id;
		}

		public async Task<bool> UpdateAsync(int cryptoId, string name, double availableQuantity, double currentPrice)
		{
			if (!_dbContext.CryptoCurrencies.Any(c => c.Id == cryptoId))
				return false; // no such user

			var crypto = await _dbContext.CryptoCurrencies.FirstAsync(c => c.Id == cryptoId);
			crypto.CurrentPrice = currentPrice;
			crypto.AvailableQuantity = availableQuantity;
			crypto.Name = name;
			await _dbContext.SaveChangesAsync();
			return true;
		}
	}
}
