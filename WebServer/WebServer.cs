using System;
using System.Net;
using System.Threading;
using System.Text;
using System.Linq;

namespace WebServer
{
    public class WebServer {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responerMethod;
        
        public WebServer(string[] prefixes, Func<HttpListenerRequest, string> method) {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException("Not supported.");
            
            // URI prefixes are required
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("Prefixes are required.");
            
            // A responder method is required
            if (method == null)
                throw new ArgumentException("Responder method is required.");
            
            foreach(var s in prefixes) {
                _listener.Prefixes.Add(s);
            }

            _responerMethod = method;
            _listener.Start();
        }

        public WebServer(Func<HttpListenerRequest, string> method, params string[] prefixes)
            : this(prefixes, method) {}

        public void Run() {
            ThreadPool.QueueUserWorkItem((o) => {
                Console.WriteLine("Web server is running...");
                try {
                    while (_listener.IsListening) {
                        ThreadPool.QueueUserWorkItem((c) => {
                            var ctx = c as HttpListenerContext;
                            try {
                                var respStr = _responerMethod(ctx.Request);
                                var buffer = Encoding.UTF8.GetBytes(respStr);
                                ctx.Response.ContentLength64 = buffer.Length;
                                ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);
                            }
                            catch {}
                            finally {
                                // Always close stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch {}
            });
        }

        public void Stop() {
            _listener.Stop();
            _listener.Close();
        }
    }
}