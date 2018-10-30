using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRAT.Communication.ResponseTypes
{
    public class SystemInfo
    {
        [JsonProperty("os")]
        public string OS { get; set; }

        [JsonProperty("hw")]
        public HardwareInfo[] HW { get; set; }
    }
}
