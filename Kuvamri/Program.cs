using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Kuvamri
{
    internal class Program
    {
        static TcpListener server;
        static void Main(string[] args)
        {
            Console.Title = "Kuvamri";

            if(args.Length != 1)
            {
                Console.WriteLine("Error: No Filename specified");
                Console.ReadKey();
                return;
            }

            try
            {
                Console.WriteLine("Starting " + args[0] + " as a server...");
                Console.WriteLine("Loading app.config");
                AppConfig.Load(Path.GetDirectoryName(args[0]));
                Console.WriteLine("Starting Server...");
                server = new TcpListener(IPAddress.Parse("127.0.0.1"), 80);
                server.Start();
                Console.WriteLine("Server Running!");
                KRIInterpreter interpreter = KRIInterpreter.Create(args[0]);
                while (true)
                {
                    var client = server.AcceptTcpClient();
                    var buffer = new byte[4096];
                    var stream = client.GetStream();
                    var length = stream.Read(buffer, 0, buffer.Length);
                    string incomingMessage = Encoding.UTF8.GetString(buffer, 0, length);
                    string url = "/";

                    try
                    {
                        url = incomingMessage.Split("\n")[0].Split(" ")[1];
                    }
                    catch (IndexOutOfRangeException) { }

                    interpreter.InterpretURL(stream, url);
                }
            }catch(Exception ex)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.Clear();
                    if (server != null)
                        server.Stop();

                    server = new TcpListener(IPAddress.Parse("127.0.0.1"), 80);
                    server.Start();
                    Console.WriteLine(":(");
                    Console.WriteLine("The Server has encountered an error and will now only send the 500 page.");
                    while (true)
                    {
                        var client = server.AcceptTcpClient();
                        var buffer = new byte[4096];
                        var stream = client.GetStream();
                        var length = stream.Read(buffer, 0, buffer.Length);

                        if (!File.Exists(AppConfig.Error500Page))
                        {
                            string css = string.Join("\n", new string[]
                            {
                            ".collapse a{",
                            "  display: block;",
                            "  background: #cdf;",
                            "}",
                            ".collapse > p{",
                            "  visibility: hidden;",
                            "}",
                            ".collapse > p:target{",
                            "  visibility: shown; ",
                            "}",
                            "details{",
                            "cursor: default;",
                            "}"
                            });

                            string err = "<h1>Error 500</h1><h3>The Server has crashed.</h3><br><details><summary>More details</summary><p>Message: " + ex.Message + "<br>Stack Trace: " + ex.StackTrace + "<br>HResult: " + ex.HResult + "</p></details><style>" + css + "</style>";

                            stream.Write(
                                Encoding.UTF8.GetBytes(
                                    "HTTP/1.0 200 OK" + Environment.NewLine
                                    + "Content-Length: " + err.Length + Environment.NewLine
                                    + "Content-Type: " + "text/html" + Environment.NewLine
                                    + Environment.NewLine
                                    + err
                                    + Environment.NewLine + Environment.NewLine));
                            continue;
                        }

                        stream.Write(
                            Encoding.UTF8.GetBytes(
                                "HTTP/1.0 200 OK" + Environment.NewLine
                                + "Content-Length: " + File.ReadAllText(AppConfig.Error500Page).Length + Environment.NewLine
                                + "Content-Type: " + "text/html" + Environment.NewLine
                                + Environment.NewLine
                                + File.ReadAllText(AppConfig.Error500Page)
                                + Environment.NewLine + Environment.NewLine));
                    }
                }catch(Exception)
                {
                    Console.WriteLine("Error 500 - Critical: fallback not successful.");
                    Console.ReadKey();
                }
            }
        }
    }
}
