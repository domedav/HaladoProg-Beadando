using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaladoProg2.DataContext.Dtos.Trade
{
	public class TradeBuyDto
	{
		public int UserId { get; set; }
		public int CryptoId { get; set; }
		public double PriceInHuf { get; set; }
	}
}
