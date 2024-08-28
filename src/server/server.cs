using CalendarCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CalendarServer
{
    public class Server
    {
        private HttpListener listener;
        private IData data;

        public Server(int port = 8080, IData? data = null)
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
                WriteContent(rsp, "nope");
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
            User user = new() { name = id.Name, password = id.Password };
            Console.WriteLine($"name: {user.name}\npass: {user.password}");

            if (!data.AuthenticateUser(user))
            {
                rsp.StatusCode = (int)HttpStatusCode.Unauthorized;
                WriteContent(rsp, "Invalid credentials.");
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
                if (rq.Url.AbsolutePath == "/User/Change/")
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
            var rq = ctx.Request;
            var rsp = ctx.Response;
            var user = JsonSerializer.Deserialize<User>(rq.InputStream);
            if(!data.CreateUser(user))
            {
                rsp.StatusCode = (int)HttpStatusCode.Forbidden;
                WriteContent(rsp, "Username is already used.");
                rsp.Close();
                return;
            }
            rsp.StatusCode = (int)HttpStatusCode.OK;
            rsp.Close();
            return;
        }

        public void ChangeUserCredentials(HttpListenerContext ctx)
        {

        }

        private void WriteContent(HttpListenerResponse rsp, string content)
        {
            var buffer = Encoding.UTF8.GetBytes(content);
            rsp.OutputStream.Write(buffer, 0, buffer.Length);
            rsp.OutputStream.Close();
            rsp.ContentLength64 = buffer.Length;
            rsp.ContentEncoding = Encoding.UTF8;
        }
    }
}
