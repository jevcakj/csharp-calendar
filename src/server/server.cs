using calendar_server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CalendarServer
{
    public class server
    {
        private HttpListener listener;
        private IData data;

        public server(int port = 8080, IData? data = null)
        {
            this.data = data ?? new FileDataStorage();
            listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{port}/");
        }

        public void StartLoop() 
        {
            listener.Start();
            while (true)
            {
                var request = listener.GetContext();
                Task.Run(() => { HandleRequest(request); });
            }

        }

        private void HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            Console.WriteLine($"handling request: {request.Url}");
        }

    }
}
