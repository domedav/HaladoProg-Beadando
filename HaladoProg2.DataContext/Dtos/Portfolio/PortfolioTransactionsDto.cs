namespace HaladoProg2.DataContext.Dtos.Portfolio;

public class PortfolioTransactionsDto
{
    public int TransactionId { get; set; }
    public DateTime TransactionDate { get; set; }
    public int CryptoId { get; set; }
    public double CryptoCount;
    public double OverallValue;
}