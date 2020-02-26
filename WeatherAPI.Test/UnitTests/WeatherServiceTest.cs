using System.IO;
using Microsoft.Extensions.Options;
using Moq;
using WeatherAPI.Services;
using WeatherAPI.Settings;
using Xunit;

namespace WeatherAPI.Test.UnitTests
{
    public class WeatherServiceTest
    {
        [Fact]
        public void GivenMockResponse_OpenWeatherMap_TransformResponse_ShouldReturnValidData()
        {
            using (StreamReader r = new StreamReader("openWeatherMapResponse.json"))
            {
                string mockResponse = r.ReadToEnd();

                IOptions<OpenWeatherMapConfig> config = Options.Create(new OpenWeatherMapConfig
                {
                    Units = UnitsEnum.Metric
                });
                OpenWeatherMapService weatherMapService = new OpenWeatherMapService(config);
                var (weatherOutput, error) = weatherMapService.TransformResponse(mockResponse);

                Assert.Empty(error);
                Assert.Equal((decimal)14.79, weatherOutput.temperature_degrees);
                Assert.Equal((decimal)(9.8 * 3.6), weatherOutput.wind_speed);

            }
        }

        [Fact]
        public void GivenMockResponse_WeatherMap_TransformResponse_ShouldReturnValidData()
        {
            using (StreamReader r = new StreamReader("weatherMapResponse.json"))
            {
                string mockResponse = r.ReadToEnd();


                var optionMock = new Mock<IOptions<WeatherStackConfig>>();
                var weatherMapService = new WeatherStackService(optionMock.Object);
                var (weatherOutput, error) = weatherMapService.TransformResponse(mockResponse);

                Assert.Empty(error);
                Assert.Equal(15, weatherOutput.temperature_degrees);
                Assert.Equal( 35, weatherOutput.wind_speed);

            }
        }
    }
}
