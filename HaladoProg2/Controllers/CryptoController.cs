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
		public async Task<IActionResult> GetAll()
		{
			var result = await _cryptoService.GetAllAsync();

			if (result == null)
				return BadRequest("Nem található adat!");

			result.ConvertAll(c => new CryptoDataDto
			{

			});
		}

		[HttpGet("{cryptoId}")]
		public async Task<IActionResult> GetAll(int cryptoId)
		{
			return await _cryptoService.Get();
		}
	}
}
