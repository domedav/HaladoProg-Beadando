using HaladoProg2.DataContext.Dtos.Profits;
using HaladoProg2.DataContext.Entities;
using HaladoProg2.Services;
using Microsoft.AspNetCore.Mvc;

namespace HaladoProg2.Controllers;

[ApiController]
[Route("api/profit")]
public class ProfitsController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICryptoService _cryptoService;

    public ProfitsController(IUserService userService, ICryptoService cryptoService)
    {
        _userService = userService;
        _cryptoService = cryptoService;
    }
    
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserProfitability(int userId)
    {
        var user = await _userService.GetIncludesAsync(userId);

        if (user == null)
            return NotFound("Nincs ilyen felhasználó!");
        
        var currentPrice = user.Wallets.Select(SumCryptoFromWalletAsync).Sum(result => result.Result);
        var purchaseValue = user.Transactions.Where(t => t.IsSelling == false).Sum(t => t.TransactionPrice * t.TransactionQuantity);
        var soldValue = user.Transactions.Where(t => t.IsSelling).Sum(t => t.TransactionPrice * t.TransactionQuantity);
        
        return Ok(new ProfitsDto()
        {
            ProfitValue = Math.Abs(currentPrice - (purchaseValue - soldValue)),
            IsProfiting = currentPrice > (purchaseValue - soldValue),
        });
    }
    
    [HttpGet("details/{userId}")]
    public async Task<IActionResult> GetUserProfitabilityDetailed(int userId)
    {
        var user = await _userService.GetIncludesAsync(userId);

        if (user == null)
            return NotFound("Nincs ilyen felhasználó!");

        var result = user.Wallets.Where(w => w.CryptoCount > 0).ToList().ConvertAll(w =>
        {
            var currentPrice = Task.FromResult(async () =>
            {
                return await SumCryptoFromWalletAsync(w);
            }).Result().Result;
            var purchaseValue = user.Transactions.Where(t => t.IsSelling == false).Where(t => t.CryptoId == w.CryptoId)
                .Sum(t => t.TransactionPrice * t.TransactionQuantity);
            var soldValue = user.Transactions.Where(t => t.IsSelling).Where(t => t.CryptoId == w.CryptoId)
                .Sum(t => t.TransactionPrice * t.TransactionQuantity);

            return new ProfitsDetailedDto()
            {
                ProfitValue = Math.Abs(currentPrice - (purchaseValue - soldValue)),
                IsProfiting = currentPrice > (purchaseValue - soldValue),
                CryptoId = w.CryptoId,
            };
        });
        return Ok(result);
    }
    
    private async Task<double> SumCryptoFromWalletAsync(Wallet w)
    {
        var crypto = await _cryptoService.GetAsync(w.CryptoId);
        if(crypto == null)
            return 0;
        return crypto.CurrentPrice * w.CryptoCount;
    }
}