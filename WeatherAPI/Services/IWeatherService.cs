using System.Threading.Tasks;
using WeatherAPI.Models;

namespace WeatherAPI.Services
{
    public interface IWeatherService
    {
        Task<(WeatherOutputModel, string)> GetCurrentWeather(string country,string city);

        (WeatherOutputModel, string) TransformResponse(string response);
        int GetPriority();
    }
}
