using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaladoProg2.DataContext.Dtos.Trade
{
	public class TradeSellDto
	{
		public int UserId { get; set; }
		public int CryptoId { get; set; }
		public double Quantity { get; set; }
	}
}
