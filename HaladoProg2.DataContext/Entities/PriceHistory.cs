using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaladoProg2.DataContext.Entities
{
	public class PriceHistory
	{
		public int Id { get; set; }
		public int CryptoId { get; set; }
		public Crypto Crypto { get; set; }
		public double CurrentPrice { get; set; }
		public DateTime Time { get; set; }
	}
}
