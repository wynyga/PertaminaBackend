using System.Threading.Tasks;
using Models.DTOs;

namespace Business.Services
{
    public interface IWeatherService
    {
        Task<WeatherResponseDTO> GetWeatherAsync(string city);
    }
}
