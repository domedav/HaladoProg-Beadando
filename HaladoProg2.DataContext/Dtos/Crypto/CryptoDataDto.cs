using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaladoProg2.DataContext.Dtos.Crypto
{
	public class CryptoDataDto
	{
		public int Id { get; set; }
		public double AvailableQuantity { get; set; }
		public double CurrentPrice { get; set; }
	}
}
