using System.Net;
using System.Text;
using System.Text.Json;

using CalendarCommon;

namespace CalendarServer
{

    /// <summary>
    /// Class representing a server that handles HTTP requests related to users and calendar events.
    /// Uses an HttpListener to listen for incoming requests and processes them based on the HTTP method and URL.
    /// </summary>
    public class Server
    {
        private HttpListener listener;
        private IData data;

        /// <summary>
        /// Constructor that initializes the server with optional data storage and a specific port.
        /// Defaults to using FileDataStorage if no data storage is provided.
        /// </summary>
        /// <param name="data">Optional IData storage implementation. Defaults to FileDataStorage.</param>
        /// <param name="port">The port on which the server will listen. Defaults to 8080.</param>
        public Server(IData? data = null, int port = 8080)
        {
            this.data = data ?? new FileDataStorage();
            listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{port}/");
            listener.AuthenticationSchemes = AuthenticationSchemes.Basic;
        }

        /// <summary>
        /// Constructor that initializes the server with a specific file path for data storage and a port.
        /// </summary>
        /// <param name="path">The path to the file data storage.</param>
        /// <param name="port">The port on which the server will listen. Defaults to 8080.</param>
        public Server(string path, int port = 8080)
        {
            data = new FileDataStorage(path);
            listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{port}/");
            listener.AuthenticationSchemes = AuthenticationSchemes.Basic;
        }

        /// <summary>
        /// Starts the server loop, continuously listens for incoming requests, and handles them asynchronously.
        /// </summary>
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

        /// <summary>
        /// Handles incoming HTTP requests based on the request method and URL.
        /// Processes requests for user registration, authentication, event handling, and credential changes.
        /// </summary>
        /// <param name="ctx">The HttpListenerContext containing the request and response.</param>
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

        /// <summary>
        /// Handles retrieving calendar events or a specific event for a user.
        /// </summary>
        /// <param name="ctx">The HttpListenerContext containing the request and response.</param>
        /// <param name="user">The user for whom the events are being retrieved.</param>
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
                List<CalendarEventBasic> calendarEvents;
                if(!data.GetEvents(dateTime, user, out calendarEvents)){
                    rsp.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }
                calendarEvents.Sort();
                content = JsonSerializer.Serialize(calendarEvents);
            }
            WriteContent(rsp, content);
            rsp.StatusCode = (int)HttpStatusCode.OK;
        }

        /// <summary>
        /// Handles saving a calendar event for a user.
        /// </summary>
        /// <param name="ctx">The HttpListenerContext containing the request and response.</param>
        /// <param name="user">The user for whom the event is being saved.</param>

        public void SaveEvent(HttpListenerContext ctx, User user)
        {
            var rq = ctx.Request;
            var rsp = ctx.Response;

            var calEvent = JsonSerializer.Deserialize<CalendarEvent>(rq.InputStream);
            int id = data.SaveEvent(calEvent, user);
            WriteContent(rsp, id);
        }

        /// <summary>
        /// Handles deleting a calendar event for a user.
        /// </summary>
        /// <param name="ctx">The HttpListenerContext containing the request and response.</param>
        /// <param name="user">The user for whom the event is being deleted.</param>
        public void DeleteEvent(HttpListenerContext ctx, User user)
        {
            var rq = ctx.Request;
            var rsp = ctx.Response;

            var path = rq.Url.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            DateTime dateTime = new(int.Parse(path[0]), int.Parse(path[1]), int.Parse(path[2]));
            int id = int.Parse(path[3]);
            data.DeleteEvent(dateTime, id, user);
        }

        /// <summary>
        /// Handles user registration by creating a new user.
        /// </summary>
        /// <param name="ctx">The HttpListenerContext containing the request and response.</param>
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

        /// <summary>
        /// Handles changing a user's credentials (username or password).
        /// </summary>
        /// <param name="ctx">The HttpListenerContext containing the request and response.</param>
        /// <param name="user">The user whose credentials are being changed.</param>
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

        /// <summary>
        /// Writes string content to the response.
        /// </summary>
        /// <param name="rsp">The HttpListenerResponse object to write to.</param>
        /// <param name="content">The string content to write.</param>
        private void WriteContent(HttpListenerResponse rsp, string content)
        {
            var buffer = Encoding.UTF8.GetBytes(content);
            rsp.OutputStream.Write(buffer, 0, buffer.Length);
            rsp.OutputStream.Close();
        }

        /// <summary>
        /// Writes integer content to the response by converting it to a string.
        /// </summary>
        /// <param name="rsp">The HttpListenerResponse object to write to.</param>
        /// <param name="content">The integer content to write.</param>
        private void WriteContent(HttpListenerResponse rsp, int content) => WriteContent(rsp, $"{content}");
    }
}
