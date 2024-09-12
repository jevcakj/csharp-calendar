using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using CalendarCommon;


namespace CalendarServer
{
    public class Server
    {
        private HttpListener listener;
        private IData data;

        public Server(IData? data = null, int port = 8080)
        {
            this.data = data ?? new FileDataStorage();
            listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{port}/");
            listener.AuthenticationSchemes = AuthenticationSchemes.Basic;
        }
        public Server(string path, int port = 8080)
        {
            data = new FileDataStorage(path);
            listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{port}/");
            listener.AuthenticationSchemes = AuthenticationSchemes.Basic;
        }

        public void StartLoop() 
        {
            listener.Start();
            Console.WriteLine("Server started.");
            Console.WriteLine($"Data source: \n\t{((FileDataStorage)data).rootDirectory}");
            Console.WriteLine("URI prefixes:");
            foreach(var prefix in listener.Prefixes)
            {
                Console.WriteLine($"\t{prefix}");
            }
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
               rq.HttpMethod != HttpMethod.Delete.Method &&
               rq.HttpMethod != HttpMethod.Head.Method)
            {
                rsp.StatusCode = (int)HttpStatusCode.BadRequest;
                WriteContent(rsp, "Bad request method");
                rsp.Close();
                Console.WriteLine("Request denied. Bad request method.");
                return;
            }

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
            
            HttpListenerBasicIdentity id = (HttpListenerBasicIdentity)ctx.User.Identity;
            User user = new() { name = id.Name, password = id.Password };
            Console.WriteLine($"User: {user.name}");

            if (!data.AuthenticateUser(user))
            {
                rsp.StatusCode = (int)HttpStatusCode.Unauthorized;
                rsp.Close();
                return;
            }

            if (rq.HttpMethod == HttpMethod.Head.Method)
            {
                rsp.StatusCode = (int)HttpStatusCode.OK;
                rsp.Close();
                return;
            }

            if (rq.HttpMethod == HttpMethod.Get.Method)
            {
                GetEvent(ctx, user);
                rsp.Close();
                return;
            }

            if(rq.HttpMethod == HttpMethod.Post.Method)
            {
                if (rq.Url.Segments[1] == "User/" && rq.Url.Segments[2] == "Change/")
                {
                    ChangeUserCredentials(ctx, user);
                }
                else
                {
                    SaveEvent(ctx, user);
                }
                rsp.Close();
                return;
            }

            if(rq.HttpMethod == HttpMethod.Delete.Method)
            {
                DeleteEvent(ctx, user);
                rsp.Close();
                return;
            }
        }

        public void GetEvent(HttpListenerContext ctx, User user)
        {
            var rq = ctx.Request;
            var rsp = ctx.Response;

            var path = rq.Url.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            DateTime dateTime = new(int.Parse(path[0]), int.Parse(path[1]), int.Parse(path[2]));
            string content;
            if(path.Length > 3)
            {
                int id = int.Parse(path[3]);
                CalendarEvent calEvent;
                if(!data.GetEvent(dateTime, id, user, out calEvent))
                {
                    rsp.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }
                content = JsonSerializer.Serialize(calEvent);
            }
            else
            {
                List<CalendarEvent> calendarEvents;
                if(!data.GetEvents(dateTime, user, out calendarEvents)){
                    rsp.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }
                content = JsonSerializer.Serialize(calendarEvents);
            }
            WriteContent(rsp, content);
            rsp.StatusCode = (int)HttpStatusCode.OK;
        }

        public void SaveEvent(HttpListenerContext ctx, User user)
        {
            var rq = ctx.Request;
            var rsp = ctx.Response;

            var calEvent = JsonSerializer.Deserialize<CalendarEvent>(rq.InputStream);
            int id = data.SaveEvent(calEvent, user);
            WriteContent(rsp, id);
        }

        public void DeleteEvent(HttpListenerContext ctx, User user)
        {
            var rq = ctx.Request;
            var rsp = ctx.Response;

            var path = rq.Url.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            DateTime dateTime = new(int.Parse(path[0]), int.Parse(path[1]), int.Parse(path[2]));
            int id = int.Parse(path[4]);
            data.DeleteEvent(dateTime, id, user);
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
                return;
            }
        }

        public void ChangeUserCredentials(HttpListenerContext ctx, User user)
        {
            var rq = ctx.Request;
            var rsp = ctx.Response;
            var newUser = JsonSerializer.Deserialize<User>(rq.InputStream);

            if (rq.Url.Segments[3] == "Name/")
            {
                if(!data.UpdateUserName(user, newUser))
                {
                    rsp.StatusCode = (int)HttpStatusCode.Forbidden;
                    WriteContent(rsp, "Username is already used.");
                    return;
                }
            }
            else if(rq.Url.Segments[3] == "Password/")
            {
                if(!data.UpdateUserPassword(user, newUser))
                {
                    rsp.StatusCode = (int)HttpStatusCode.BadRequest;
                    WriteContent(rsp, "Unable to change password.");
                    return;
                }
            }
            else
            {
                rsp.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }
            rsp.StatusCode = (int)HttpStatusCode.OK;
        }

        private void WriteContent(HttpListenerResponse rsp, string content)
        {
            var buffer = Encoding.UTF8.GetBytes(content);
            rsp.OutputStream.Write(buffer, 0, buffer.Length);
            rsp.OutputStream.Close();
        }

        private void WriteContent(HttpListenerResponse rsp, int content) => WriteContent(rsp, $"{content}");
    }
}
