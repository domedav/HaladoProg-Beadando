namespace HaladoProg2.DataContext.Dtos.Pricing;

public class PricingHistoryDto
{
    public int PriceHistoryId { get; set; }
    public int CryptoId { get; set; }
    public double Price { get; set; }
    public DateTime Time { get; set; }
}