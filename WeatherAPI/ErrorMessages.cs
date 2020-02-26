namespace WeatherAPI
{
    public static class ErrorMessages
    {
        public static string CannotGetWeatherData(string source, string query) =>
            $"Cannot get weather data from {source} for query: {query}";

        public const string CannotTransformResponse = "Cannot transform the response";

        public const string AllWeatherServiceFailed = "All weather service have failed";

        public const string UncaughtError = "There is an error with our system. Please contact the administrors!";
        public const string InvalidWindSpeed = "Invalid value for wind speed";
        public const string InvalidTemperature = "Invalid value for temperature";


    }
}
