namespace HaladoProg2.DataContext.Dtos.Portfolio;

public class PortfolioWalletDto
{
    public int WalletId { get; set; }
    public double WalletValue { get; set; }
    public int CryptoId { get; set; }
    public string CryptoName { get; set; }
    public double CryptoCount { get; set; }
}