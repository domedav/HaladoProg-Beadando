namespace HaladoProg2.DataContext.Dtos.Portfolio;

public class PortfolioDto
{
    public string UserName { get; set; }
    public int WalletsCount { get; set; }
    public int BuyTransactionsCount { get; set; }
    public int SellTransactionsCount { get; set; }
    public double TotalNetWorth { get; set; }
    public List<PortfolioWalletDto>? PortfolioWallets { get; set; }
    public List<PortfolioTransactionsDto>? PortfolioTransactionsBuy { get; set; }
    public List<PortfolioTransactionsDto>? PortfolioTransactionsSell { get; set; }
}