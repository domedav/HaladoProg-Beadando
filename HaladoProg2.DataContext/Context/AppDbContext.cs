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
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Wallet> Wallets { get; set; }
		public DbSet<Crypto> CryptoCurrencies { get; set; }
		public DbSet<Transaction> Transactions { get; set; }
		public DbSet<PriceHistory> PriceHistories { get; set; }

		public void SeedData()
		{
			if (!CryptoCurrencies.Any()) // we have 15 pre defined crypto
			{
				CryptoCurrencies.AddRange(
					new Crypto { Name = "Bitcoin (BTC)", AvailableQuantity = 100, CurrentPrice = 30323608 },
					new Crypto { Name = "Ethereum (ETH)", AvailableQuantity = 100, CurrentPrice = 570616 },
					new Crypto { Name = "Tether (USDT)", AvailableQuantity = 50, CurrentPrice = 359 },
					new Crypto { Name = "XRP (XRP)", AvailableQuantity = 50, CurrentPrice = 740 },
					new Crypto { Name = "Solana (SOL)", AvailableQuantity = 50, CurrentPrice = 48039 },
					new Crypto { Name = "USD Coin (USDC)", AvailableQuantity = 50, CurrentPrice = 359 },
					new Crypto { Name = "Dogecoin (DOGE)", AvailableQuantity = 50, CurrentPrice = 56.55 },
					new Crypto { Name = "TRON (TRX)", AvailableQuantity = 50, CurrentPrice = 90.15 },
					new Crypto { Name = "Cardano (ADA)", AvailableQuantity = 50, CurrentPrice = 225.40 },
					new Crypto { Name = "Wrapped Bitcoin (WBTC)", AvailableQuantity = 50, CurrentPrice = 23148253 },
					new Crypto { Name = "Avalanche (AVAX)", AvailableQuantity = 20, CurrentPrice = 6907 },
					new Crypto { Name = "Chainlink (LINK)", AvailableQuantity = 20, CurrentPrice = 4320 },
					new Crypto { Name = "Stellar (XLM)", AvailableQuantity = 20, CurrentPrice = 86.24 },
					new Crypto { Name = "Toncoin (TON)", AvailableQuantity = 20, CurrentPrice = 2059 },
					new Crypto { Name = "Shiba Inu (SHIB)", AvailableQuantity = 20, CurrentPrice = 0.0043 }
				);
				SaveChanges();
			}
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

			modelBuilder.Entity<Crypto>()
				.Property(c => c.CurrentPrice)
				.HasColumnType("float"); // maps to float

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
