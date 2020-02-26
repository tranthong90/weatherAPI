using System;
using System.Threading.Tasks;
using WeatherAPI.Models;
using WeatherAPI.Settings;

namespace WeatherAPI.Services
{
    public abstract class BaseWeatherService : IWeatherService
    {
        private readonly BaseWeatherSourceConfig _config;

        protected BaseWeatherService(BaseWeatherSourceConfig config)
        {
            _config = config;
        }

        public virtual Task<(WeatherOutputModel, string)> GetCurrentWeather(string country, string city)
        {
            throw new NotImplementedException();
        }

        public virtual (WeatherOutputModel, string) TransformResponse(string response)
        {
            throw new NotImplementedException();
        }

        public int GetPriority()
        {
            return _config.PriorityOrder;
        }
    }
}
