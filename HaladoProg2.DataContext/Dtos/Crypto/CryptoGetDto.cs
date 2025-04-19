using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HaladoProg2.DataContext.Dtos.PriceHistory;
using HaladoProg2.DataContext.Dtos.Transaction;
using HaladoProg2.DataContext.Dtos.Wallet;
using HaladoProg2.DataContext.Entities;

namespace HaladoProg2.DataContext.Dtos.Crypto
{
	public class CryptoGetDto
	{
		public int Id { get; set; }
		public double AvailableQuantity { get; set; }
		public double CurrentPrice { get; set; }
		public List<WalletDataDto> ContainingWallets { get; set; }
		public List<TransactionDataDto> Transactions { get; set; }
		public List<PriceHistoryDataDto> PriceHistories { get; set; }
	}
}
