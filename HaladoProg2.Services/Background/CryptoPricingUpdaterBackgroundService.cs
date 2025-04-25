using Microsoft.Extensions.Hosting;

namespace HaladoProg2.Services.Background;

public class CryptoPricingUpdaterBackgroundService : BackgroundService
{
    private readonly IPriceHistoryService _priceHistoryService;
    private readonly ICryptoService _cryptoService;

    public CryptoPricingUpdaterBackgroundService(IPriceHistoryService priceHistoryService, ICryptoService cryptoService)
    {
        _priceHistoryService = priceHistoryService;
        _cryptoService = cryptoService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested) // while it is not cancelled, we run it
        {
            var randomWaitInterval = 30 + (Random.Shared.NextDouble() * 30); //30 to 60 random
            await Task.Delay(TimeSpan.FromSeconds(randomWaitInterval), stoppingToken); // we wait for the interval

            var result = await UpdateCryptoPricing();
            Console.WriteLine(!result ? "Crypto pricing update failed" : "Crypto pricing update complete");
        }
        Console.WriteLine("Crypto pricing update service stopped");
    }

    private async Task<bool> UpdateCryptoPricing()
    {
        var all = await _cryptoService.GetAllAsync();
        
        if (all.Count <= 0) // nothing to update
            return false;
        
        all.ForEach(async c =>
        {
            var newPrice = Double.Lerp(
                -(c.CurrentPrice / 2),
                c.CurrentPrice / 2,
                (StockMarketDrasticPriceRandom(Random.Shared.NextDouble() * 2) + 1) / 2); // the method gives a value between -1 and 1, but we can only lerp between 0 and 1
            
            await _cryptoService.UpdateAsync(c.Id, c.Name, c.AvailableQuantity, newPrice);
            await _priceHistoryService.CreateAsync(c.Id, newPrice, DateTime.UtcNow);
        });
        return true;
    }

    private double StockMarketDrasticPriceRandom(double x) // desmos equivalent (paste in desmos.com): f\left(x\right)=\left(\left(\frac{\left(\left|x\right|\cdot\log\left(x\right)\right)}{\exp\left(x\right)}\right)^{0.2}\right)\cdot1.56\ +\ 0.1
    {
        x = double.Clamp(x, 0.001, 1.999);
        
        var result = Math.Abs(x) * Math.Log(x);
        result /= Math.Exp(x);
        result = Math.Pow(result, 0.2);
        return double.Clamp(result * 1.56d + 0.1, -1, 1);
    }
}