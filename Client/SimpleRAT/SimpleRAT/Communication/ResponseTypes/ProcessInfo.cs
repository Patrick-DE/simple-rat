using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRAT.Communication.ResponseTypes
{
    public class ProcessInfo
    {
        [JsonProperty("pid")]
        public int PID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("modules")]
        public string[] Modules { get; set; }

        [JsonProperty("start")]
        public DateTime Start { get; set; }
    }
}
