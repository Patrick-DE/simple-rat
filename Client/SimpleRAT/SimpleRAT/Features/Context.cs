using SimpleRAT.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRAT.Features
{
    public class Context
    {
        public Request Request { get; private set; }
        public Response Response { get; private set; }

        public Context(Request request)
        {
            Request = request;
            Response = new Response(request);
        }
    }
}
