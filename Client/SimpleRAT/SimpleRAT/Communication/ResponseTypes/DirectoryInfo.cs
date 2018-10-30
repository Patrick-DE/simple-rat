using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRAT.Communication.ResponseTypes
{
    public class DirectoryInfo
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("directories")]
        public string[] Directories { get; set; }

        [JsonProperty("files")]
        public string[] Files { get; set; }
    }
}
