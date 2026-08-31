using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Models.DTOs;

namespace Business.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherResponseDTO> GetWeatherAsync(string city)
        {
            try
            {
                // wttr.in mengembalikan format JSON jika kita tambahkan ?format=j1
                var response = await _httpClient.GetAsync($"{city}?format=j1");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new KeyNotFoundException($"Kota '{city}' tidak ditemukan.");
                }

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                
                using (JsonDocument doc = JsonDocument.Parse(content))
                {
                    var currentCondition = doc.RootElement
                        .GetProperty("current_condition")[0];

                    string tempString = currentCondition.GetProperty("temp_C").GetString() ?? "0";
                    string desc = currentCondition.GetProperty("weatherDesc")[0].GetProperty("value").GetString() ?? "Unknown";
                    string humidityString = currentCondition.GetProperty("humidity").GetString() ?? "0";

                    return new WeatherResponseDTO
                    {
                        City = city,
                        Temperature = int.Parse(tempString),
                        Description = desc,
                        Humidity = int.Parse(humidityString)
                    };
                }
            }
            catch (TaskCanceledException ex)
            {
                // TaskCanceledException dilempar ketika HttpClient mencapai batas Timeout
                throw new TimeoutException("Koneksi ke API Cuaca terlalu lama (Timeout).", ex);
            }
            catch (JsonException ex)
            {
                // JsonException dilempar jika response dari API tidak berbentuk JSON yang valid (atau formatnya berubah)
                throw new InvalidOperationException("Response dari API Cuaca tidak valid.", ex);
            }
        }
    }
}
