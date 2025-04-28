using HaladoProg2.DataContext.Dtos.Transaction;
using HaladoProg2.DataContext.Dtos.Wallet;

namespace HaladoProg2.DataContext.Dtos.User
{
	public class UserDataDto
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public List<WalletDataDto> Wallets { get; set; }
		public List<TransactionDataDto> Transactions { get; set; }
	}
}
