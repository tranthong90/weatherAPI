using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WeatherAPI.Settings
{
    public abstract class BaseWeatherSourceConfig
    {
        public int PriorityOrder { get; set; }
        public string BaseURL { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public UnitsEnum Units { get; set; }
    }
}
