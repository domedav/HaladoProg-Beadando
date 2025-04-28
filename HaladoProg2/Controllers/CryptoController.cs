using HaladoProg2.DataContext.Dtos.Crypto;
using HaladoProg2.DataContext.Entities;
using HaladoProg2.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HaladoProg2.Controllers
{
	[ApiController]
	[Route("api/cryptos")]
	public class CryptoController : ControllerBase
	{
		private readonly ICryptoService _cryptoService;
		private readonly IUserService _userService;
		private readonly IWalletService _walletService;


		public CryptoController(ICryptoService cryptoService, IUserService userService, IWalletService walletService)
		{
			_cryptoService = cryptoService;
			_userService = userService;
			_walletService = walletService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllCryptoAsync()
		{
			var result = await _cryptoService.GetAllAsync();

			if (result.IsNullOrEmpty())
				return NotFound("Nem található adat!");

			var convert = result.ConvertAll(c => new CryptoDataDto
			{
				Id = c.Id,
				AvailableQuantity = c.AvailableQuantity,
				CurrentPrice = c.CurrentPrice,
			});
			return Ok(convert);
		}

		[HttpGet("{cryptoId}")]
		public async Task<IActionResult> GetCryptoAsync(int cryptoId)
		{
			var result = await _cryptoService.GetAsync(cryptoId);

			if (result == null)
				return NotFound("Nem található kriptó ezzel az Id-vel!");

			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> CreateCryptoAsync([FromBody] CryptoCreateDto cryptoCreateDto)
		{
			if (cryptoCreateDto.CurrentPrice < 0 ||
			    cryptoCreateDto.AvailableQuantity < 0 ||
			    cryptoCreateDto.Name.Trim() == string.Empty)
				return BadRequest("A kért művelet nem valósítható meg, mert érvénytelen értéket tartalmaz!");

			var result = await _cryptoService.CreateAsync(cryptoCreateDto.Name, cryptoCreateDto.AvailableQuantity, cryptoCreateDto.CurrentPrice);
			if (!result)
				return BadRequest("Nem sikerült a kért kriptót létrehozni!");

			var id = await _cryptoService.GetCryptoIdByNameAsync(cryptoCreateDto.Name);
			if (id == null)
				return NotFound("Nem található kriptó ezzel az Id-vel!");

			var crypto = await _cryptoService.GetAsync((int)id);
			if (crypto == null)
				return NotFound("Nem található kriptó ezzel az Id-vel!");

			var users = await _userService.GetAllUsersIdAsync();
			foreach (var item in users)
			{
				await _walletService.CreateAsync(item, crypto.Id, 0); // new crypto, new wallet for all users where they can store it
			}

			return Ok(result);
		}

		[HttpDelete("{cryptoId}")]
		public async Task<IActionResult> DeleteCryptoAsync(int cryptoId)
		{
			var result = await _cryptoService.DeleteAsync(cryptoId);
			return Ok(result);
		}
	}
}
