using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace WebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var ws = new WebServer(SendResponse, "http://localhost:8080/headers/");
            ws.Run();
            Console.WriteLine("Press any key to quit.");
            Console.ReadKey();
            ws.Stop();
        }

        public static string SendResponse(HttpListenerRequest req) {
            var coll = req.Headers;
            var headers1 = coll.AllKeys;
            var str = "";

            for (var i = 0; i < headers1.Length; i++) {
                str += $"<li>{headers1[i]}</li>";

                var headers2 = coll.GetValues(headers1[i]);
                for (var j = 0; j < headers2.Length; j++) {
                    str += $"<ul><li>{headers2[j]}</li></ul>";
                }
            }
            return string.Format("<HTML><BODY>Hello there!<br><br><ul>{0}</ul></BODY></HTML>", str);
        }
    }
}
