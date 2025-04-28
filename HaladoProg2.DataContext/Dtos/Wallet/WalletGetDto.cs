namespace HaladoProg2.DataContext.Dtos.Wallet
{
	public class WalletGetDto
	{
		public double TotalMoneyHuf { get; set; }
		public double TotalCryptoTypesCount { get; set; }
		public List<WalletGetCryptoDto> Wallets { get; set; }
	}
}
