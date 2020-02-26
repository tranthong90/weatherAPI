using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WeatherAPI.CQRS.Queries;

namespace WeatherAPI.Controllers.v1
{
    [Route("v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class WeatherController : ControllerBase
    {
        private readonly IMediator _mediator;
        public WeatherController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ResponseCache(Duration = 3, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> GetCurrentWeather(string city = "Melbourne", string country = "AU")
        {
            if (string.IsNullOrEmpty(city))
                return BadRequest();

            var (weatherOutputModel, error) = await _mediator.Send(new GetCurrentWeatherQuery(country, city));

            return string.IsNullOrEmpty(error) ? Ok(weatherOutputModel) : StatusCode(500, error);
        }
    }
}