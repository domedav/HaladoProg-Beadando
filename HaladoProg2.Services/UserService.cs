using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HaladoProg2.DataContext.Context;
using HaladoProg2.DataContext.Dtos.User;
using HaladoProg2.DataContext.Entities;

using Microsoft.EntityFrameworkCore;

namespace HaladoProg2.Services
{
	public interface IUserService
	{
		Task<bool> CreateAsync(string username, string email, string password);
		Task<User?> GetAsync(int userId);
		Task<User?> GetIncludesAsync(int userId);
		Task<bool> UpdateAsync(int userId, string username, string email, string password);
		Task<bool> DeleteAsync(int userId);
		Task<bool> SetMoney(int userId, double money);
		Task<bool> ModifyMoney(int userId, double money);
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

			if (_dbContext.Users.Where(u => u.Email == email).Any()) // email not unique
				return false;

			_dbContext.Users.Add(
				new User
				{
					Username = username,
					Email = email,
					Password = password,
					Wallets = [],
					Transactions = [],
				});

			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteAsync(int userId)
		{
			if (!_dbContext.Users.Where(u => u.Id == userId).Any())
				return false; // no such user

			_dbContext.Users.Remove(await _dbContext.Users.Where(u => u.Id == userId).FirstAsync());

			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<User?> GetAsync(int userId)
		{
			var user = await _dbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
			return user;
		}
		
		public async Task<User?> GetIncludesAsync(int userId)
		{
			var user = await _dbContext.Users
				.Include(u => u.Wallets)
				.Include(u => u.Transactions)
				.Where(u => u.Id == userId).FirstOrDefaultAsync();
			return user;
		}

		public async Task<bool> SetMoney(int userId, double money)
		{
			var user = await _dbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
			if (user == null)
				return false;

			user.UserMoney = money;
			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ModifyMoney(int userId, double money)
		{
			var user = await _dbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
			if (user == null)
				return false;

			user.UserMoney += money;
			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<bool> UpdateAsync(int userId, string username, string email, string password)
		{
			if (!_dbContext.Users.Where(u => u.Id == userId).Any())
				return false; // no such user

			var user = await _dbContext.Users.Where(u => u.Id == userId).FirstAsync();
			user.Username = username;
			user.Email = email;
			user.Password = password;
			await _dbContext.SaveChangesAsync();
			return true;
		}
	}
}
