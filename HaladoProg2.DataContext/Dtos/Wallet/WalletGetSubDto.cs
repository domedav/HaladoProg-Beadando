using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaladoProg2.DataContext.Dtos.Wallet
{
	public class WalletGetSubDto
	{
		public int Id { get; set; }
		public int CryptoId { get; set; }
		public double CryptoCount { get; set; }
		public double Value { get; set; }
	}
}
