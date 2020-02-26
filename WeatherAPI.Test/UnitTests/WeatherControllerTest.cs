using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WeatherAPI.Controllers.v1;
using WeatherAPI.CQRS.Queries;
using WeatherAPI.Models;
using Xunit;

namespace WeatherAPI.Test.UnitTests
{
    public class WeatherControllerTest
    {
        [Fact]
        public void GivenMockResponse_GetCurrentWeather_ShouldReturnValidData()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<GetCurrentWeatherQuery>(), It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync((new WeatherOutputModel(5, 23), string.Empty));

            WeatherController controller = new WeatherController(mediatorMock.Object);

            var result = controller.GetCurrentWeather("Melbourne").Result as OkObjectResult;
            var responseValue = result?.Value as WeatherOutputModel;
            Assert.NotNull(responseValue);
            Assert.Equal(5, responseValue.wind_speed);
            Assert.Equal(23, responseValue.temperature_degrees);
        }
    }
}
