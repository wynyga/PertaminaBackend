using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Business.Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("{city}")]
        public async Task<IActionResult> GetWeather(string city)
        {
            try
            {
                var weather = await _weatherService.GetWeatherAsync(city);
                return Ok(weather);
            }
            catch (KeyNotFoundException ex)
            {
                // 404 Not Found (Kota tidak ada)
                return NotFound(new { Message = ex.Message });
            }
            catch (TimeoutException)
            {
                // 504 Gateway Timeout (Layanan cuaca lambat)
                return StatusCode(504, new { Message = "Layanan cuaca sedang tidak merespons (Timeout). Silakan coba lagi nanti." });
            }
            catch (InvalidOperationException)
            {
                // 502 Bad Gateway (Layanan cuaca mengembalikan data sampah)
                return StatusCode(502, new { Message = "Menerima data yang tidak valid dari layanan cuaca." });
            }
            catch (HttpRequestException)
            {
                // 502 Bad Gateway (Layanan cuaca error 500 atau mati)
                return StatusCode(502, new { Message = "Gagal terhubung ke layanan cuaca saat ini." });
            }
            catch (Exception)
            {
                // 500 Internal Server Error (Fallback aman)
                return StatusCode(500, new { Message = "Terjadi kesalahan internal yang tidak terduga." });
            }
        }
    }
}
