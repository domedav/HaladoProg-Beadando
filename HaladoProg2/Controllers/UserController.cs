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

		public UserController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDto userRegisterDto)
		{
			if (userRegisterDto.Username.Trim() == string.Empty ||
				userRegisterDto.Email.Trim() == string.Empty ||
				userRegisterDto.Password.Trim() == string.Empty)
				return BadRequest("Egy vagy több adat nem lett kitöltve!");

			var result = await _userService.CreateAsync(userRegisterDto.Username, userRegisterDto.Email, userRegisterDto.Password);

			if (!result)
				return BadRequest("Ez az email már regisztrálva van!");

			return Ok(result);
		}

		[HttpGet("{userId}")]
		public async Task<IActionResult> GetUser(int userId)
		{
			var user = await _userService.GetAsync(userId);

			if (user == null)
				return NotFound("Ez a felhasználó nem létezik!");

			var userDto = new UserDataDto // create a dto from the user data
			{
				Id = userId,
				Username = user.Username,
				Email = user.Email,
				Wallets = user.Wallets == null ? [] : user.Wallets.ConvertAll(c => new WalletDataDto
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
				Transactions = user.Transactions == null ? [] : user.Transactions.ConvertAll(c => new TransactionDataDto
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
		public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserUpdateDto userUpdateDto)
		{
			if (userUpdateDto.Username.Trim() == string.Empty ||
				userUpdateDto.Email.Trim() == string.Empty ||
				userUpdateDto.Password.Trim() == string.Empty)
				return BadRequest("Egy vagy több adat nem lett kitöltve!");

			var result = await _userService.UpdateAsync(userId, userUpdateDto.Username, userUpdateDto.Email, userUpdateDto.Password);
			if (!result)
				return BadRequest("Nem sikerült a felhasználói adatokat módosítani!");

			return Ok(result);
		}

		[HttpDelete("{userId}")]
		public async Task<IActionResult> DeleteUser(int userId)
		{
			var result = await _userService.DeleteAsync(userId);
			if (!result)
				return BadRequest("Nem sikerült a felhasználót törölni!");

			return Ok(result);
		}
	}
}
