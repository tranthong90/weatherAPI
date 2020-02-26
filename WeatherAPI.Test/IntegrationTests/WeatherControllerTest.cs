using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using WeatherAPI.Models;
using Xunit;

namespace WeatherAPI.Test.IntegrationTests
{
    public class WeatherControllerTest
    {
        private readonly string _contentRoot = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "..", "..", "..", "..", "..", "WeatherAPI", "WeatherAPI");

        [Fact]
        public async Task GivenValidRequest_GetCurrentWeather_ShouldReturnValidData()
        {
            var builder = new WebHostBuilder()
                .UseContentRoot(_contentRoot)
                .UseEnvironment("Development")
                .UseStartup<Startup>();

            TestServer testServer = new TestServer(builder);
            HttpClient client = testServer.CreateClient();

            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Get, "v1/weather?city=Melbourne");


            var response = await client.SendAsync(postRequest);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseString);

            var weatherOutput = JsonConvert.DeserializeObject<WeatherOutputModel>(responseString);

            Assert.NotNull(weatherOutput);
        }
    }
}
