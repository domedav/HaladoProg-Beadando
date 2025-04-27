using HaladoProg2.DataContext.Context;
using HaladoProg2.DataContext.Entities;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HaladoProg2.Services.Background;

public class CryptoPricingUpdaterBackgroundService : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;

	public CryptoPricingUpdaterBackgroundService(IServiceProvider serviceProvider)
    {
		_serviceProvider = serviceProvider;
	}

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested) // while it is not cancelled, we run it
        {
            var randomWaitInterval = 30 + (Random.Shared.NextDouble() * 60); //30 to 60 random
            await Task.Delay(TimeSpan.FromSeconds(randomWaitInterval), stoppingToken); // we wait for the interval

			using (var scope = _serviceProvider.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
				var result = await UpdateCryptoPricingAsync(dbContext);
				Console.WriteLine(!result ? "Crypto pricing update failed" : "Crypto pricing update complete");
			}
		}
        Console.WriteLine("Crypto pricing update service stopped");
    }

	private async Task<bool> UpdateCryptoPricingAsync(AppDbContext dbContext)
    {
        var all = GetAllCrypto(dbContext);
        
        if (all.Count <= 0) // nothing to update
            return false;

		foreach (var c in all)
		{
			var newPrice = SimulateNextPrice(c.CurrentPrice);

			var update = UpdateCrypto(c.Id, newPrice, dbContext);
            if (!update)
                continue;
			CreatePriceHistory(c.Id, newPrice, DateTime.UtcNow, dbContext);
		}
		await dbContext.SaveChangesAsync();
		return true;
    }

	private double SimulateNextPrice(double currentPrice)
	{
		double nextPrice = Double.Lerp(currentPrice * 0.98, currentPrice * 1.02, Random.Shared.NextDouble());

		// prevent collapse
		if (nextPrice <= 0)
			nextPrice = currentPrice;

		return Math.Round(nextPrice, 5);
	}

	private List<Crypto> GetAllCrypto(AppDbContext dbContext)
    {
        return dbContext.CryptoCurrencies.ToList();
    }

    private bool UpdateCrypto(int cid, double newPrice, AppDbContext dbContext)
    {
        var crypto = dbContext.CryptoCurrencies.Where(c => c.Id == cid).FirstOrDefault();
        if (crypto == null)
            return false;

        crypto.CurrentPrice = newPrice;
        return true;
    }

    private void CreatePriceHistory(int cryptoId, double price, DateTime date, AppDbContext dbContext)
    {
        dbContext.PriceHistories.Add(new PriceHistory { CryptoId = cryptoId, CurrentPrice = price, Time = date });
    }
}