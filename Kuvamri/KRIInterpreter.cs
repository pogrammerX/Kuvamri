using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Kuvamri
{
    public class KRIInterpreter
    {
        public string[] KRILines;

        public static KRIInterpreter Create(string kriPath)
        {
            KRIInterpreter interpreter = new KRIInterpreter();
            interpreter.KRILines = File.ReadAllLines(kriPath);
            return interpreter;
        }

        public void InterpretURL(NetworkStream stream, string url)
        {
            foreach (string line in KRILines)
            {
                switch (line.Split(".")[0])
                {
                    case "server":
                        switch (line.Split(".")[1].Split("(")[0])
                        {
                            case "listen":
                                string serverGetArg1 = line.Split("(")[1].Split(",")[0].Replace("\"", "");
                                string serverGetStreamVarName = line.Split("(")[2];
                                serverGetStreamVarName = serverGetStreamVarName.Split(")")[0];
                                string code = line.Split("{")[1].Substring(1, line.Split("{")[1].Length - 3);
                                if(serverGetArg1 == url)
                                {
                                    if (code.Split("->")[0] == serverGetStreamVarName + ".html")
                                    {
                                        stream.Write(
                                            Encoding.UTF8.GetBytes(
                                                "HTTP/1.0 200 OK" + Environment.NewLine
                                                + "Content-Length: " + code.Split("->")[1].Length + Environment.NewLine
                                                + "Content-Type: " + "text/html" + Environment.NewLine
                                                + Environment.NewLine
                                                + code.Split("->")[1]
                                                + Environment.NewLine + Environment.NewLine));
                                    }
                                    if (code.Split("->")[0] == serverGetStreamVarName + ".text")
                                    {
                                        stream.Write(
                                            Encoding.UTF8.GetBytes(
                                                "HTTP/1.0 200 OK" + Environment.NewLine
                                                + "Content-Length: " + code.Split("->")[1].Length + Environment.NewLine
                                                + "Content-Type: " + "text/plain" + Environment.NewLine
                                                + Environment.NewLine
                                                + code.Split("->")[1]
                                                + Environment.NewLine + Environment.NewLine));
                                    }
                                }
                                break;
                        }
                        break;
                }
            }
        }
    }
}
