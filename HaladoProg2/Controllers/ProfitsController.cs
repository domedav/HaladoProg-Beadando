using HaladoProg2.DataContext.Dtos.Profits;
using HaladoProg2.DataContext.Entities;
using HaladoProg2.Services;
using Microsoft.AspNetCore.Mvc;

namespace HaladoProg2.Controllers
{
    [ApiController]
    [Route("api/profit")]
    public class ProfitsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWalletService _walletService;
        private readonly ITransactionService _transactionService;

        public ProfitsController(IUserService userService, ITransactionService transactionService, IWalletService walletService)
        {
            _userService = userService;
            _transactionService = transactionService;
            _walletService = walletService;
        }
        
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserProfitabilityAsync(int userId)
        {
            var user = await _userService.GetAsync(userId);

            if (user == null)
                return NotFound("Nincs ilyen felhasználó!");

            var currentPrice = await _userService.GetAllWalletWorthAsync(userId);
            var purchaseValue = await _transactionService.GetSumUserBuyingAsync(userId);
            var soldValue = await _transactionService.GetSumUserSellingAsync(userId);
            
            return Ok(new ProfitsDto()
            {
                ProfitValue = Math.Abs(currentPrice - (purchaseValue - soldValue)),
                IsProfiting = currentPrice > (purchaseValue - soldValue),
            });
        }
        
        [HttpGet("details/{userId}")]
        public async Task<IActionResult> GetUserProfitabilityDetailedAsync(int userId)
        {
            var user = await _userService.GetAsync(userId);

            if (user == null)
                return NotFound("Nincs ilyen felhasználó!");

            var currentPrice = await _userService.GetAllWalletWorthAsync(userId);
            var wallets = await _walletService.GetUserWalletsNonEmptyAsync(userId);
            List<ProfitsDetailedDto> listResult = [];
            foreach (var item in wallets)
            {
                var purchaseValue = await _transactionService.GetSumUserBuyingCryptoAsync(userId, item.CryptoId);
                var soldValue = await _transactionService.GetSumUserSellingCryptoAsync(userId, item.CryptoId);

                listResult.Add(new ProfitsDetailedDto()
                {
                    ProfitValue = Math.Abs(currentPrice - (purchaseValue - soldValue)),
                    IsProfiting = currentPrice > (purchaseValue - soldValue),
                    CryptoId = item.CryptoId,
                });
            }
            return Ok(listResult);
        }
    }   
}