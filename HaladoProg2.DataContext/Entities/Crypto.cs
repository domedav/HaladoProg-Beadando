namespace HaladoProg2.DataContext.Entities
{
	public class Crypto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public double AvailableQuantity { get; set; }
		public double CurrentPrice { get; set; }
		public List<Wallet> ContainingWallets { get; set; }
		public List<Transaction> Transactions { get; set; }
		public List<PriceHistory> PriceHistories { get; set; }
	}
}
