using HaladoProg2.DataContext.Dtos.Crypto;
using HaladoProg2.DataContext.Dtos.Transaction;
using HaladoProg2.DataContext.Dtos.User;
using HaladoProg2.DataContext.Dtos.Wallet;
using HaladoProg2.Services;
using Microsoft.AspNetCore.Mvc;

namespace HaladoProg2.Controllers
{
	[ApiController]
	[Route("api/users")]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly ICryptoService _cryptoService;
		private readonly IWalletService _walletService;
		private readonly ITransactionService _transactionService;

		public UserController(IUserService userService, ICryptoService cryptoService, IWalletService walletService, ITransactionService transactionService)
		{
			_userService = userService;
			_cryptoService = cryptoService;
			_walletService = walletService;
			_transactionService = transactionService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> RegisterUserAsync([FromBody] UserRegisterDto userRegisterDto)
		{
			var result = await _userService.CreateAsync(userRegisterDto.Username, userRegisterDto.Email, userRegisterDto.Password);

			if (!result)
				return BadRequest("Ez az email már regisztrálva van! Vagy hibás a megadott regisztrációs adatlap!");

			var cryptos = await _cryptoService.GetAllAsync();
			foreach (var crypto in cryptos)
			{
				var id = await _userService.GetIdByEmailAsync(userRegisterDto.Email);
				await _walletService.CreateAsync((int)id!, crypto.Id, 0); // empty wallet for the given crypto
			}
			
			return Ok(result);
		}

		[HttpGet("{userId}")]
		public async Task<IActionResult> GetUserAsync(int userId)
		{
			var user = await _userService.GetAsync(userId);

			if (user == null)
				return NotFound("Ez a felhasználó nem létezik!");

			var userWallets = await _walletService.GetUserWallets(userId);
			var userTrans = await _transactionService.GetUserAllAsync(userId);
			var userDto = new UserDataDto // create a dto from the user data
			{
				Id = userId,
				Username = user.Username,
				Email = user.Email,
				Wallets = userWallets.ConvertAll(c => new WalletDataDto
				{
					Id = c.Id,
					CryptoCount = c.CryptoCount,
					CryptoData = new CryptoDataDto
					{
						Id = c.Crypto.Id,
						AvailableQuantity = c.Crypto.AvailableQuantity,
						CurrentPrice = c.Crypto.CurrentPrice,
					}
				}),
				Transactions = userTrans.ConvertAll(c => new TransactionDataDto
				{
					Id= c.Id,
					CryptoId = c.CryptoId,
					TransactionPrice = c.TransactionPrice,
					TransactionQuantity = c.TransactionQuantity,
				})
			};

			return Ok(userDto);
		}

		[HttpPut("{userId}")]
		public async Task<IActionResult> UpdateUserAsync(int userId, [FromBody] UserUpdateDto userUpdateDto)
		{
			var result = await _userService.UpdateAsync(userId, userUpdateDto.Username, userUpdateDto.Email, userUpdateDto.Password);
			if (!result)
				return BadRequest("Nem sikerült a felhasználói adatokat módosítani!");

			return Ok(result);
		}

		[HttpDelete("{userId}")]
		public async Task<IActionResult> DeleteUserAsync(int userId)
		{
			var result = await _userService.DeleteAsync(userId);
			if (!result)
				return BadRequest("Nem sikerült a felhasználót törölni!");

			return Ok(result);
		}
		
		[HttpPut("money/{userId}")]
		public async Task<IActionResult> AddMoney(int userId, [FromBody] UserMoneyDto userMoneyDto)
		{
			var user = await _userService.GetAsync(userId);
			if (user == null)
				return BadRequest("Nincs ilyen user!");

			var result = await _userService.SetMoneyAsync(userId, userMoneyDto.UserMoney);

			return Ok(result);
		}

	}
}
