using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HaladoProg2.DataContext.Context;

namespace HaladoProg2.Services
{
	public interface ITransactionService
	{

	}

	public class TransactionService : ITransactionService
	{
		private readonly AppDbContext _dbContext;
		public TransactionService(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}
	}
}
