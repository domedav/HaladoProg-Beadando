using System.Security.Cryptography;
using System.Text;
using HaladoProg2.DataContext.Context;
using HaladoProg2.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace HaladoProg2.Services
{
	public interface IUserService
	{
		Task<bool> CreateAsync(string username, string email, string password);
		Task<User?> GetAsync(int userId);
		Task<int?> GetIdByEmailAsync(string email);
		Task<List<int>> GetAllUsersIdAsync();
		Task<User?> GetIncludesAsync(int userId);
		Task<bool> UpdateAsync(int userId, string username, string email, string password);
		Task<bool> DeleteAsync(int userId);
		Task<bool> SetMoneyAsync(int userId, double money);
		Task<bool> ModifyMoneyAsync(int userId, double money);
	}

	public class UserService : IUserService
	{
		private readonly AppDbContext _dbContext;
		public UserService(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}
		public async Task<bool> CreateAsync(string username, string email, string password)
		{
			if (username.Trim() == string.Empty ||
				email.Trim() == string.Empty ||
				password.Trim() == string.Empty) // missing data
				return false;

			if (await _dbContext.Users.AnyAsync(u => u.Email == email)) // email not unique
				return false;

			_dbContext.Users.Add(
				new User
				{
					Username = username,
					Email = email,
					Password = EncryptPassword(password),
					Wallets = [],
					Transactions = [],
					UserMoney = 10000 // default money
				});

			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteAsync(int userId)
		{
			if (!(await _dbContext.Users.AnyAsync(u => u.Id == userId)))
				return false; // no such user

			_dbContext.Users.Remove(await _dbContext.Users.FirstAsync(u => u.Id == userId));

			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<User?> GetAsync(int userId)
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
			return user;
		}

		public async Task<int?> GetIdByEmailAsync(string email)
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
			if (user == null)
				return null;
			return user.Id;
		}

		public async Task<List<int>> GetAllUsersIdAsync()
		{
			return _dbContext.Users.ToList().ConvertAll(c => c.Id);
		}

		public async Task<User?> GetIncludesAsync(int userId)
		{
			var user = await _dbContext.Users
				.Include(u => u.Wallets)
					.ThenInclude(w => w.Crypto)
				.Include(u => u.Transactions)
				.Where(u => u.Id == userId).FirstOrDefaultAsync();
			return user;
		}

		public async Task<bool> SetMoneyAsync(int userId, double money)
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
			if (user == null)
				return false;

			user.UserMoney = money;
			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ModifyMoneyAsync(int userId, double money)
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
			if (user == null)
				return false;

			user.UserMoney += money;
			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<bool> UpdateAsync(int userId, string username, string email, string password)
		{
			if (!(await _dbContext.Users.AnyAsync(u => u.Id == userId)))
				return false; // no such user

			var user = await _dbContext.Users.FirstAsync(u => u.Id == userId);
			user.Username = username;
			user.Email = email;
			user.Password = EncryptPassword(password);
			await _dbContext.SaveChangesAsync();
			return true;
		}

		private string EncryptPassword(string raw)
		{
			var sha256 = SHA256.Create();
			byte[] hashValue;
			UTF8Encoding objUtf8 = new UTF8Encoding();
			hashValue = sha256.ComputeHash(objUtf8.GetBytes(raw));
			return Convert.ToBase64String(hashValue);
		}
	}
}
