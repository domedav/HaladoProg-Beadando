using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaladoProg2.DataContext.Dtos.Wallet
{
	public class WalletUpdateDto
	{
		public int WalletId { get; set; }
		public double NewAmount { get; set; }
	}
}
