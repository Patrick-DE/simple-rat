using SimpleRAT.Communication;
using SimpleRAT.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRAT
{
    public class RequestHandler
    {
        public BaseFeature[] Features { get; private set; }

        public RequestHandler(BaseFeature[] features)
        {
            Features = features;
        }

        public Response Handle(Request request)
        {
            if (request.Command == Commands.NONE) {
                var res = new Response(request);
                res.AddError(Commands.NONE, "Nice try. Now select a valid command stupid.");
                return res;
            }
            var context = new Context(request);
            foreach (var feature in Features)
            {
                if (feature.CanHandle(context.Request.Command))
                    feature.Handle(context);
            }
            return context.Response;
        }
    }
}
