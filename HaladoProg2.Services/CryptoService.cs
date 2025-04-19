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
	public interface ICryptoService
	{
		Task<List<Crypto>> GetAllAsync();
		Task<Crypto?> GetAsync(int crpytoId);
		Task<bool> CreateAsync(string name, double availableQuantity, double currentPrice);
		Task<bool> UpdateAsync(int crpytoId, string name, double availableQuantity, double currentPrice);
		Task<bool> DeleteAsync(int crpytoId);
	}

	public class CryptoService : ICryptoService
	{
		private readonly AppDbContext _dbContext;
		public CryptoService(AppDbContext dbContext)
		{
			_dbContext = dbContext;
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

		public async Task<bool> DeleteAsync(int crpytoId)
		{
			if (!_dbContext.CryptoCurrencies.Where(c => c.Id == crpytoId).Any())
				return false; // no such crypto

			_dbContext.CryptoCurrencies.Remove(await _dbContext.CryptoCurrencies.Where(c => c.Id == crpytoId).FirstAsync());

			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<List<Crypto>> GetAllAsync()
		{
			return _dbContext.CryptoCurrencies.AsEnumerable().ToList();
		}

		public async Task<Crypto?> GetAsync(int crpytoId)
		{
			var crypto = await _dbContext.CryptoCurrencies.Where(c => c.Id == crpytoId).FirstAsync();
			return crypto;
		}

		public async Task<bool> UpdateAsync(int crpytoId, string name, double availableQuantity, double currentPrice)
		{
			if (!_dbContext.CryptoCurrencies.Where(c => c.Id == crpytoId).Any())
				return false; // no such user

			var crypto = await _dbContext.CryptoCurrencies.Where(c => c.Id == crpytoId).FirstAsync();
			crypto.CurrentPrice = currentPrice;
			crypto.AvailableQuantity = availableQuantity;
			crypto.Name = name;
			await _dbContext.SaveChangesAsync();
			return true;
		}
	}
}
