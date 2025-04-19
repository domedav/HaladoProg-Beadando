using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaladoProg2.DataContext.Dtos.Transaction
{
	public class TransactionDataDto
	{
		public int Id { get; set; }
		public int CryptoId { get; set; }
		public double TransactionQuantity { get; set; }
		public double TransactionPrice { get; set; }
	}
}
