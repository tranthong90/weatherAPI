using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using WeatherAPI.Models;
using WeatherAPI.Settings;

namespace WeatherAPI.Services
{
    public class WeatherStackService : BaseWeatherService
    {
        private readonly WeatherStackConfig _config;

        public WeatherStackService(IOptions<WeatherStackConfig> config) : base(config.Value)
        {
            _config = config.Value;
        }

        public override async Task<(WeatherOutputModel, string)> GetCurrentWeather(string country, string city)
        {
            try
            {
                using var client = new HttpClient();
                var response =
                    await client.GetAsync($"{_config.BaseURL}?access_key={_config.AccessToken}&query={city}&units={GetUnitsParametrer(_config.Units)}");

                if (!response.IsSuccessStatusCode)
                    return (null, ErrorMessages.CannotGetWeatherData("WeatherStack", city));

                var content = await response.Content.ReadAsStringAsync();

                return TransformResponse(content);
            }
            catch (Exception)
            {
                return (null, ErrorMessages.UncaughtError);
            }

        }

        private string GetUnitsParametrer(UnitsEnum unitsEnum)
        {
            return unitsEnum switch
            {
                UnitsEnum.Metric => "m",
                UnitsEnum.Fahrenheit => "f",
                UnitsEnum.Scientific => "s",
                _ => "m"
            };
        }
        public override (WeatherOutputModel, string) TransformResponse(string response)
        {
            try
            {
                var myJObject = JObject.Parse(response);
                string windSpeed = myJObject.SelectToken("$.current.wind_speed").Value<string>();
                string temperature = myJObject.SelectToken("$.current.temperature").Value<string>();
                if (string.IsNullOrEmpty(windSpeed) || string.IsNullOrEmpty(temperature))
                    return (null, ErrorMessages.CannotTransformResponse);

                if (!Decimal.TryParse(windSpeed, out var windSpeedDecimal))
                    return (null, ErrorMessages.InvalidWindSpeed);

                if (!Decimal.TryParse(temperature, out var temperatureDecimal))
                    return (null, ErrorMessages.InvalidTemperature);

                return (new WeatherOutputModel(windSpeedDecimal, temperatureDecimal), string.Empty);
            }
            catch (Exception)
            {
                return (null, ErrorMessages.UncaughtError);
            }
        }
    }
}
