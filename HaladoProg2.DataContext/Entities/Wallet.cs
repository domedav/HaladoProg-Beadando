namespace HaladoProg2.DataContext.Entities
{
	public class Wallet
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public User User { get; set; }
		public int CryptoId { get; set; }
		public Crypto Crypto { get; set; }
		public double CryptoCount { get; set; }
	}
}
