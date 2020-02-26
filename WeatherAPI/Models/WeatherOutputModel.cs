namespace WeatherAPI.Models
{
    public class WeatherOutputModel
    {
        // ReSharper disable once InconsistentNaming
        public decimal wind_speed { get; set; }

        // ReSharper disable once InconsistentNaming
        public decimal temperature_degrees { get; set; }

        public WeatherOutputModel(decimal windSpeed, decimal temperature)
        {
            wind_speed = windSpeed;
            temperature_degrees = temperature;
        }
    }
}
