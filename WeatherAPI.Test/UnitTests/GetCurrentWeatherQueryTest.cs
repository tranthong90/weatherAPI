using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using WeatherAPI.CQRS.Queries;
using WeatherAPI.Models;
using WeatherAPI.Services;
using WeatherAPI.Settings;
using Xunit;

namespace WeatherAPI.Test.UnitTests
{
    public class GetCurrentWeatherQueryTest
    {
        [Fact]
        public async Task Given2GoodWeatherSources_Handle_UseTheTopPriorityOne()
        {
            GetCurrentWeatherQuery query = new GetCurrentWeatherQuery("AU", "Melbourne");

            IOptions<WeatherStackConfig> weatherStackconfig = Options.Create(new WeatherStackConfig
            {
                Units = UnitsEnum.Metric,
                PriorityOrder = 1
            });
            var weatherStackService = new Mock<WeatherStackService>(weatherStackconfig);
            weatherStackService.Setup(w => w.GetCurrentWeather("AU", "Melbourne"))
                .ReturnsAsync((new WeatherOutputModel(5, 23), string.Empty));
            IOptions<OpenWeatherMapConfig> config = Options.Create(new OpenWeatherMapConfig
            {
                Units = UnitsEnum.Metric,
                PriorityOrder = 2
            });
            OpenWeatherMapService openWeatherMapService = new OpenWeatherMapService(config);

            List<IWeatherService> weatherServices = new List<IWeatherService>
            {
                weatherStackService.Object, openWeatherMapService
            };

            var memoryCache = new Mock<IMemoryCacheService>();
            memoryCache.Setup(m => m.SetCache(It.IsAny<string>(), It.IsAny<object>()));

            GetCurrentWeatherQueryHandler queryHandler = new GetCurrentWeatherQueryHandler(weatherServices, memoryCache.Object);
            await queryHandler.Handle(query, CancellationToken.None);
            //assert
            weatherStackService.Verify(w => w.GetCurrentWeather("AU", "Melbourne"));
        }

        [Fact]
        public async Task Given1BadWeatherSources_Handle_ShouldUseTheOtherSource()
        {
            GetCurrentWeatherQuery query = new GetCurrentWeatherQuery("AU", "Melbourne");

            IOptions<OpenWeatherMapConfig> config = Options.Create(new OpenWeatherMapConfig
            {
                BaseURL = "WillFailURL",
                Units = UnitsEnum.Metric,
                PriorityOrder = 1
            });
            OpenWeatherMapService openWeatherMapService = new OpenWeatherMapService(config);

            IOptions<WeatherStackConfig> weatherStackconfig = Options.Create(new WeatherStackConfig
            {
                Units = UnitsEnum.Metric,
                PriorityOrder = 2
            });
            var weatherStackService = new Mock<WeatherStackService>(weatherStackconfig);
            weatherStackService.Setup(w => w.GetCurrentWeather("AU", "Melbourne"))
                .ReturnsAsync((new WeatherOutputModel(5, 23), string.Empty));



            List<IWeatherService> weatherServices = new List<IWeatherService>
            {
                weatherStackService.Object, openWeatherMapService
            };

            var memoryCache = new Mock<IMemoryCacheService>();
            memoryCache.Setup(m => m.SetCache(It.IsAny<string>(), It.IsAny<object>()));

            GetCurrentWeatherQueryHandler queryHandler = new GetCurrentWeatherQueryHandler(weatherServices, memoryCache.Object);
            await queryHandler.Handle(query, CancellationToken.None);
            //assert
            weatherStackService.Verify(w => w.GetCurrentWeather("AU", "Melbourne"));
        }

        [Fact]
        public async Task GivenAllBadWeatherSources_Handle_ShouldGetFromMemoryCache()
        {
            GetCurrentWeatherQuery query = new GetCurrentWeatherQuery("AU", "Melbourne");

            IOptions<OpenWeatherMapConfig> config = Options.Create(new OpenWeatherMapConfig
            {
                BaseURL = "WillFailURL",
                Units = UnitsEnum.Metric,
                PriorityOrder = 1
            });
            OpenWeatherMapService openWeatherMapService = new OpenWeatherMapService(config);

            IOptions<WeatherStackConfig> weatherStackConfig = Options.Create(new WeatherStackConfig
            {
                BaseURL = "WillFailURL",
                Units = UnitsEnum.Metric,
                PriorityOrder = 2
            });
            var weatherStackService = new WeatherStackService(weatherStackConfig);

            List<IWeatherService> weatherServices = new List<IWeatherService>
            {
                weatherStackService, openWeatherMapService
            };

            var memoryCache = new Mock<IMemoryCacheService>();
            memoryCache.Setup(m => m.GetValueFromCache<WeatherOutputModel>(It.IsAny<string>())).Returns(new WeatherOutputModel(5, 23));

            GetCurrentWeatherQueryHandler queryHandler = new GetCurrentWeatherQueryHandler(weatherServices, memoryCache.Object);
            var (weatherOutput, error) = await queryHandler.Handle(query, CancellationToken.None);

            //assert
            Assert.Empty(error);
            Assert.Equal(5, weatherOutput.wind_speed);
            Assert.Equal(23, weatherOutput.temperature_degrees);

        }
    }
}
