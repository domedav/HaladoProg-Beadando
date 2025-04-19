using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaladoProg2.DataContext.Dtos.PriceHistory
{
	public class PriceHistoryDataDto
	{
		public int Id { get; set; }
		public int CryptoId { get; set; }
		public double CurrentPrice { get; set; }
		public DateTime Time { get; set; }
	}
}
