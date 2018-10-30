using Newtonsoft.Json;
using SimpleRAT.Communication.ResponseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRAT.Communication
{
    public class Response
    {
        public Response()
        {
            ID = 0;
            Content = new Dictionary<string, object>();
            Errors = new List<ErrorMessage>();
            Content["meta.timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public Response(Request req) : this()
        {
            ID = req.ID;
        }

        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("content")]
        public Dictionary<string, object> Content { get; set; }

        [JsonProperty("errors")]
        public List<ErrorMessage> Errors { get; set; }

        public void AddError(Commands command, string message)
        {
            Errors.Add(new ErrorMessage() { Command = command.ToString(), Message = message });
        }
    }
}
