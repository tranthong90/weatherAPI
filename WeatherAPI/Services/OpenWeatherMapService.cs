using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using WeatherAPI.Models;
using WeatherAPI.Settings;

namespace WeatherAPI.Services
{
    public class OpenWeatherMapService : BaseWeatherService
    {
        private readonly OpenWeatherMapConfig _config;

        public OpenWeatherMapService(IOptions<OpenWeatherMapConfig> config) : base(config.Value)
        {
            _config = config.Value;
        }


        public override async Task<(WeatherOutputModel, string)> GetCurrentWeather(string country, string city)
        {
            try
            {
                using var client = new HttpClient();
                var response =
                    await client.GetAsync($"{_config.BaseURL}?appid={_config.AppId}&q={city},{country}&units={_config.Units.ToString()}");

                if (!response.IsSuccessStatusCode)
                    return (null, ErrorMessages.CannotGetWeatherData("OpenWeatherMap", $"{city},{country}"));

                var content = await response.Content.ReadAsStringAsync();

                return TransformResponse(content);
            }
            catch (Exception)
            {
                return (null, ErrorMessages.UncaughtError);
            }
        }

        public override (WeatherOutputModel, string) TransformResponse(string response)
        {
            try
            {
                var myJObject = JObject.Parse(response);
                string windSpeed = myJObject.SelectToken("$.wind.speed").Value<string>();
                string temperature = myJObject.SelectToken("$.main.temp").Value<string>();
                if (string.IsNullOrEmpty(windSpeed) || string.IsNullOrEmpty(temperature))
                    return (null, ErrorMessages.CannotTransformResponse);

                if (!Decimal.TryParse(windSpeed, out var windSpeedDecimal))
                    return (null, ErrorMessages.InvalidWindSpeed);

                if (!Decimal.TryParse(temperature, out var temperatureDecimal))
                    return (null, ErrorMessages.InvalidTemperature);

                if (_config.Units == UnitsEnum.Metric)
                    windSpeedDecimal *= (decimal)3.6;

                return (new WeatherOutputModel(windSpeedDecimal, temperatureDecimal), string.Empty);
            }
            catch (Exception)
            {
                return (null, ErrorMessages.UncaughtError);
            }
        }

    }
}
