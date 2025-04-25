using HaladoProg2.DataContext.Dtos.Crypto;
using HaladoProg2.DataContext.Dtos.Pricing;
using HaladoProg2.Services;
using Microsoft.AspNetCore.Mvc;

namespace HaladoProg2.Controllers;

[ApiController]
[Route("api/crypto")]
public class CryptoPricingController : ControllerBase
{
    private readonly IPriceHistoryService _priceHistoryService;
    private readonly ICryptoService _cryptoService;

    public CryptoPricingController(IPriceHistoryService priceHistoryService, ICryptoService cryptoService)
    {
        _priceHistoryService = priceHistoryService;
        _cryptoService = cryptoService;
    }
    
    [HttpPut("price")]
    public async Task<IActionResult> UpdateSetCrypto([FromBody] CryptoUpdateDto cryptoUpdateDto)
    {
        var result = await _cryptoService.GetAsync(cryptoUpdateDto.Id);
        
        if (result == null)
            return NotFound("Nem található ezzel az ID-vel crypto!");

        var res = await _cryptoService.UpdateAsync(cryptoUpdateDto.Id, result.Name, result.AvailableQuantity, cryptoUpdateDto.NewPrice);
        res &= await _priceHistoryService.CreateAsync(cryptoUpdateDto.Id, cryptoUpdateDto.NewPrice, DateTime.Now);
        
        return Ok(res);
    }
    
    [HttpGet("price/history/{cryptoId}")]
    public async Task<IActionResult> GetPriceHistory(int cryptoId)
    {
        var result = await _cryptoService.GetIncludesAsync(cryptoId);
        
        if (result == null)
            return NotFound("Nem található ezzel az ID-vel crypto!");

        var res = result.PriceHistories.ConvertAll(c => new PricingHistoryDto()
        {
            PriceHistoryId = c.Id,
            CryptoId = c.CryptoId,
            Price = c.CurrentPrice,
            Time = c.Time
        }).OrderByDescending(p => p.Time);
        
        return Ok(res);
    }
}