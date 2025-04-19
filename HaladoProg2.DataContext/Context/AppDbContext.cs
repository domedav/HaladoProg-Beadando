using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HaladoProg2.DataContext.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HaladoProg2.DataContext.Context
{
	public class AppDbContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Wallet> Wallets { get; set; }
		public DbSet<Crypto> CryptoCurrencies { get; set; }
		public DbSet<Transaction> Transactions { get; set; }
		public DbSet<PriceHistory> PriceHistories { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var connectionString = "Server=(local);Database=CryptoDb_JBPWXQ;Trusted_Connection=True;TrustServerCertificate=True;";
			optionsBuilder.UseSqlServer(connectionString);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>()
				.HasKey(u => u.Id);

			modelBuilder.Entity<User>()
				.HasIndex(u => u.Email)
				.IsUnique();

			modelBuilder.Entity<Wallet>()
				.HasKey(w => w.Id);

			modelBuilder.Entity<Wallet>()
				.HasOne(w => w.User)
				.WithMany(u => u.Wallets)
				.HasForeignKey(w => w.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Wallet>()
				.HasOne(w => w.Crypto)
				.WithMany(c => c.ContainingWallets)
				.HasForeignKey(w => w.CryptoId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Crypto>()
				.HasKey(c => c.Id);

			modelBuilder.Entity<Transaction>()
				.HasKey(t => t.Id);

			modelBuilder.Entity<Transaction>()
				.HasOne(t => t.User)
				.WithMany(u => u.Transactions)
				.HasForeignKey(t => t.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Transaction>()
				.HasOne(t => t.Crypto)
				.WithMany(c => c.Transactions)
				.HasForeignKey(t => t.CryptoId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<PriceHistory>()
				.HasKey(ph => ph.Id);

			modelBuilder.Entity<PriceHistory>()
				.HasOne(ph => ph.Crypto)
				.WithMany(c => c.PriceHistories)
				.HasForeignKey(ph => ph.CryptoId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
