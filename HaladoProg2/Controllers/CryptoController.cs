using HaladoProg2.DataContext.Dtos.Crypto;
using HaladoProg2.DataContext.Entities;
using HaladoProg2.Services;

using Microsoft.AspNetCore.Mvc;

namespace HaladoProg2.Controllers
{
	[ApiController]
	[Route("api/cryptos")]
	public class CryptoController : ControllerBase
	{
		private readonly ICryptoService _cryptoService;

		public CryptoController(ICryptoService cryptoService)
		{
			_cryptoService = cryptoService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllCrypto()
		{
			var result = await _cryptoService.GetAllAsync();

			if (result == null)
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
		public async Task<IActionResult> GetCrypto(int cryptoId)
		{
			var result = await _cryptoService.GetAsync(cryptoId);

			if (result == null)
				return NotFound("Nem található kriptó ezzel az Id-vel!");

			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> CreateCrypto([FromBody] CryptoCreateDto cryptoCreateDto)
		{
			if(cryptoCreateDto.CurrentPrice < 0 ||
				cryptoCreateDto.AvailableQuantity < 0 ||
				cryptoCreateDto.Name.Trim() == string.Empty)
				return BadRequest("A kért művelet nem valósítható meg, mert érvénytelen értéket tartalmaz!");

			var result = await _cryptoService.CreateAsync(cryptoCreateDto.Name, cryptoCreateDto.AvailableQuantity, cryptoCreateDto.CurrentPrice);

			return Ok(result);
		}

		[HttpDelete("{cryptoId}")]
		public async Task<IActionResult> CreateCrypto(int cryptoId)
		{
			var result = await _cryptoService.DeleteAsync(cryptoId);
			return Ok(result);
		}
	}
}
