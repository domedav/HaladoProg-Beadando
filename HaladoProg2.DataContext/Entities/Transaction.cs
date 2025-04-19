using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaladoProg2.DataContext.Entities
{
	public class Transaction
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public User User { get; set; }
		public int CryptoId { get; set; }
		public Crypto Crypto { get; set; }
		public double TransactionQuantity { get; set; }
		public double TransactionPrice { get; set; }
	}
}
