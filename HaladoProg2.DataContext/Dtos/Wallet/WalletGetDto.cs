using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaladoProg2.DataContext.Dtos.Wallet
{
	public class WalletGetDto
	{
		public double TotalMoneyHuf { get; set; }
		public double TotalCryptoTypesCount { get; set; }
		public List<WalletGetSubDto> Wallets { get; set; }
	}
}
