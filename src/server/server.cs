using CalendarCommon;
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
            listener.AuthenticationSchemes = AuthenticationSchemes.Basic;
        }

        public void StartLoop() 
        {
            listener.Start();
            Console.WriteLine("Server started.");
            while (true)
            {
                var request = listener.GetContext();
                Task.Run(() => { HandleRequest(request); });
            }

        }

        private void HandleRequest(HttpListenerContext ctx)
        {
            var rq = ctx.Request;
            var rsp = ctx.Response;
            
            if (rq.HttpMethod != HttpMethod.Get.Method &&
               rq.HttpMethod != HttpMethod.Post.Method &&
               rq.HttpMethod != HttpMethod.Delete.Method)
            {
                rsp.StatusCode = (int)HttpStatusCode.BadRequest;
                var buffer = Encoding.UTF8.GetBytes("nope");
                rsp.OutputStream.Write(buffer, 0, buffer.Length);
                rsp.OutputStream.Close();
                rsp.ContentLength64 = buffer.Length;
                rsp.Close();
                Console.WriteLine("bad request");
                return;
            }

            Console.WriteLine($"handling request: {rq.Url}");

            if(rq.Url == null)
            {
                rsp.StatusCode = (int)HttpStatusCode.BadRequest;
                rsp.Close();
                return;
            }

            if(rq.HttpMethod == HttpMethod.Post.Method && rq.Url.AbsolutePath == "/User/Register/")
            {
                RegisterUser(ctx);
                rsp.Close();
                return;
            }
            
            var id = (HttpListenerBasicIdentity)ctx.User.Identity;
            User user = new() { Name = id.Name, Password = id.Password };
            Console.WriteLine($"name: {user.Name}\npass: {user.Password}");

            if (!data.AuthenticateUser(user))
            {
                rsp.StatusCode = (int)HttpStatusCode.Unauthorized;
                rsp.Close();
                return;
            }

            if (rq.HttpMethod == HttpMethod.Get.Method)
            {
                GetEvent(ctx);
                rsp.Close();
                return;
            }

            if(rq.HttpMethod == HttpMethod.Post.Method)
            {
                if (rq.Url.AbsolutePath == "/User/Change")
                {
                    ChangeUserCredentials(ctx);
                }
                else
                {
                    SaveEvent(ctx);
                }
                return;
            }

            if(rq.HttpMethod == HttpMethod.Delete.Method)
            {
                DeleteEvent(ctx);
                return;
            }
        }

        public void GetEvent(HttpListenerContext ctx)
        {

        }

        public void SaveEvent(HttpListenerContext ctx)
        {

        }

        public void DeleteEvent(HttpListenerContext ctx)
        {

        }

        public void RegisterUser(HttpListenerContext ctx)
        {

        }

        public void ChangeUserCredentials(HttpListenerContext ctx)
        {

        }
    }
}
