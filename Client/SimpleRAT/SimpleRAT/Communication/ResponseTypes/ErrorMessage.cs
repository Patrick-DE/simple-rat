using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRAT.Communication.ResponseTypes
{
    public class ErrorMessage
    {
        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
