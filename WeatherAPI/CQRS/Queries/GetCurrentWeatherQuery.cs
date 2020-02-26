using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WeatherAPI.Models;
using WeatherAPI.Services;

namespace WeatherAPI.CQRS.Queries
{
    public class GetCurrentWeatherQuery : IRequest<(WeatherOutputModel, string)>
    {
        public GetCurrentWeatherQuery(string country, string city)
        {
            Country = country;
            City = city;
        }

        public string City { get; }
        public string Country { get; }
    }

    public class GetCurrentWeatherQueryHandler : IRequestHandler<GetCurrentWeatherQuery, (WeatherOutputModel, string)>
    {
        private readonly IEnumerable<IWeatherService> _weatherServices;
        private readonly IMemoryCacheService _memoryCache;
        public GetCurrentWeatherQueryHandler(IEnumerable<IWeatherService> weatherServices, IMemoryCacheService memoryCache)
        {
            _weatherServices = weatherServices;
            _memoryCache = memoryCache;
        }

        public async Task<(WeatherOutputModel, string)> Handle(GetCurrentWeatherQuery request, CancellationToken cancellationToken)
        {
            var orderedList = _weatherServices.OrderBy(x => x.GetPriority()).ToList();
            foreach (var weatherService in orderedList)
            {
                var (weatherOutputModel, error) = await weatherService.GetCurrentWeather(request.Country, request.City);

                if (!string.IsNullOrEmpty(error)) //if has error, continue to the next weather service
                    continue;

                //update the cache
                _memoryCache.SetCache(CacheKeys.WeatherOutput, weatherOutputModel);

                return (weatherOutputModel, error);
            }

            //all the weather service failed, get value from cache
            var cachedValue = _memoryCache.GetValueFromCache<WeatherOutputModel>(CacheKeys.WeatherOutput);

            return (cachedValue, string.Empty);
        }
    }
}
