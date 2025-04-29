using HaladoProg2.DataContext.Context;
using HaladoProg2.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace HaladoProg2.Services
{
	public interface IWalletService
	{
		Task<bool> CreateAsync(int userId, int cryptoId, double cryptoCount);
		Task<Wallet?> GetAsync(int walletId);
		Task<Wallet?> GetUserWalletWithCryptoAsync(int userId, int cryptoId);
		Task<List<Wallet>> GetUserWalletsNonEmptyAsync(int userId);
		Task<List<Wallet>> GetUserWallets(int userId);
		Task<bool> UpdateAsync(int walletId, int userId, int cryptoId, double cryptoCount);
		Task<bool> DeleteAsync(int walletId);
	}

	public class WalletService : IWalletService
	{
		private readonly AppDbContext _dbContext;
		public WalletService(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<bool> CreateAsync(int userId, int cryptoId, double cryptoCount)
		{
			if (cryptoCount < 0)
				return false; // cant create negative crypto

			_dbContext.Wallets.Add(new Wallet
			{
				UserId = userId,
				CryptoId = cryptoId,
				CryptoCount = cryptoCount
			});
			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteAsync(int walletId)
		{
			if (!(await _dbContext.Wallets.AnyAsync(w => w.Id == walletId)))
				return false; // no such wallet

			_dbContext.Wallets.Remove(await _dbContext.Wallets.FirstAsync(w => w.Id == walletId));

			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<Wallet?> GetAsync(int walletId)
		{
			var wallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.Id == walletId);
			return wallet;
		}

		public async Task<Wallet?> GetUserWalletWithCryptoAsync(int userId, int cryptoId)
		{
			var user = await _dbContext.Users.Include(w => w.Wallets).ThenInclude(w => w.Crypto).FirstOrDefaultAsync(u => u.Id == userId);
			if (user == null)
				return null;
			var result = await user.Wallets.AsQueryable().FirstOrDefaultAsync(w => w.CryptoId == cryptoId);
			return result;
		}

		public async Task<List<Wallet>> GetUserWalletsNonEmptyAsync(int userId)
		{
			var user = await _dbContext.Users.Include(w => w.Wallets).ThenInclude(w => w.Crypto).FirstOrDefaultAsync(u => u.Id == userId);
			if (user == null)
				return [];
			var result = user.Wallets.Where(w => w.CryptoCount > 0).ToList();
			return result;
		}

		public async Task<List<Wallet>> GetUserWallets(int userId)
		{
			var user = await _dbContext.Users.Include(w => w.Wallets).ThenInclude(w => w.Crypto).FirstOrDefaultAsync(u => u.Id == userId);
			if (user == null)
				return [];
			var result = user.Wallets.ToList();
			return result;
		}

		public async Task<bool> UpdateAsync(int walletId, int userId, int cryptoId, double cryptoCount)
		{
			if (!(await _dbContext.Wallets.AnyAsync(w => w.Id == walletId)))
				return false; // no such wallet

			var wallet = await _dbContext.Wallets.FirstAsync(w => w.Id == walletId);
			wallet.UserId = userId;
			wallet.CryptoId = cryptoId;
			wallet.CryptoCount = cryptoCount;
			await _dbContext.SaveChangesAsync();
			return true;
		}
	}
}
