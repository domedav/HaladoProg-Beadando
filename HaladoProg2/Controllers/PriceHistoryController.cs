using HaladoProg2.DataContext.Dtos.Pricing;
using HaladoProg2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HaladoProg2.Controllers
{
    [ApiController]
    [Route("api/history")]
    public class PriceHistoryController : ControllerBase
    {
        private readonly IPriceHistoryService _priceHistoryService;
		
        public PriceHistoryController(IPriceHistoryService priceHistoryService)
        {
            _priceHistoryService = priceHistoryService;
        }
        
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPriceHistoryAsync()
        {
            var history = await _priceHistoryService.GetAllAsync();

            if (history.IsNullOrEmpty())
                return NotFound("Nem található adat!");

            var convert = history.ConvertAll(c => new PricingHistoryDto()
            {
                PriceHistoryId = c.Id,
                CryptoId = c.CryptoId,
                Price = c.CurrentPrice,
                Time = c.Time,
            });
            return Ok(convert);
        }
        
        [HttpGet("{historyId}")]
        public async Task<IActionResult> GetAllPriceHistoryAsync(int historyId)
        {
            var history = await _priceHistoryService.GetAsync(historyId);

            if (history == null)
                return NotFound("Nincs ezzel az id-vel crypto history");

            var result = new PricingHistoryDto()
            {
                PriceHistoryId = history.Id,
                CryptoId = history.CryptoId,
                Price = history.CurrentPrice,
                Time = history.Time,
            };
            return Ok(result);
        }
    }   
}