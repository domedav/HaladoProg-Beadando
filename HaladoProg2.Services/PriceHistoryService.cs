using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HaladoProg2.DataContext.Context;

namespace HaladoProg2.Services
{
	public interface IPriceHistoryService
	{

	}

	public class PriceHistoryService : IPriceHistoryService
	{
		private readonly AppDbContext _dbContext;
		public PriceHistoryService(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}
	}
}
