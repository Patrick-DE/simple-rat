using Newtonsoft.Json;

namespace SimpleRAT.Communication.ResponseTypes
{
    public class HardwareInfo
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}