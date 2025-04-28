using HaladoProg2.DataContext.Dtos.Crypto;

namespace HaladoProg2.DataContext.Dtos.Wallet
{
	public class WalletDataDto
	{
		public int Id { get; set; }
		public double CryptoCount { get; set; }
		public CryptoDataDto CryptoData { get; set; }
	}
}
