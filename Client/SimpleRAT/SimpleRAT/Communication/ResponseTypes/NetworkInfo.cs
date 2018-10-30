using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRAT.Communication.ResponseTypes
{
    public class NetworkInfo
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("mac")]
        public string Mac;

        [JsonProperty("status")]
        public string Status;

        [JsonProperty("gatewayAddresses")]
        public string[] GatewayAddresses;

        [JsonProperty("dnsAddresses")]
        public string[] DnsAddresses;

        [JsonProperty("dhcpAddresses")]
        public string[] DhcpAddresses;

        [JsonProperty("unicastAddresses")]
        public string[] UnicastAddresses;

        [JsonProperty("multicastAddresses")]
        public string[] MulticastAddresses;

        [JsonProperty("anycastAddresses")]
        public string[] AnycastAddresses;
    }
}
