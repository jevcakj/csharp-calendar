using CalendarCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CalendarClient
{
    public class HttpConnection : IConnection
    {
        private User defaultUser;
        private User user;
        private HttpClient client;
        public HttpConnection(User defaultUser)
        {
            this.defaultUser = defaultUser;
            this.user = defaultUser;
            client = new HttpClient();
            var authenticationString = $"{defaultUser.name}:{defaultUser.password}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(authenticationString));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        }

        public bool CreateUser(User user)
        {
            UriBuilder uriBuilder = new();
            uriBuilder.Path = "/User/Register/";
            uriBuilder.Host = "localhost";
            uriBuilder.Port = 8080;
            var content = JsonContent.Create(user);
            var responseTask = client.PostAsync(uriBuilder.Uri, content);
            responseTask.Wait();
            var response = responseTask.Result;
            //TODO nejak vratit duvod failu
            //var str = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public bool ChangeUser(User newUser)
        {
            UriBuilder uriBuilder = new();
            uriBuilder.Path = user.name == newUser.name ? "/User/Change/Password/" : "/User/Change/Name/";
            uriBuilder.Host = "localhost";
            uriBuilder.Port = 8080;
            var content = JsonContent.Create(newUser);
            var responseTask = client.PostAsync(uriBuilder.Uri, content);
            responseTask.Wait();
            var response = responseTask.Result;
            if (response.IsSuccessStatusCode)
            {
                user = newUser;
                var authenticationString = $"{user.name}:{user.password}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(authenticationString));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
                return true;
            }
            return false;
        }

        public bool SaveEvent(CalendarEvent calendarEvent)
        {
            UriBuilder uriBuilder = new();
            uriBuilder.Path = "/Post";
            uriBuilder.Host = "localhost";
            uriBuilder.Port = 8080;
            var content = JsonContent.Create(calendarEvent);
            var responseTask = client.PostAsync(uriBuilder.Uri, content);
            responseTask.Wait();
            var response = responseTask.Result;
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            var idTask = response.Content.ReadAsStringAsync();
            var id = idTask.Result;
            calendarEvent.id = int.Parse(id);
            return true;
        }

        public bool DeleteEvent(DateTime date, int id)
        {
            UriBuilder uriBuilder = new();
            uriBuilder.Path = $"/{date.Year}/{date.Month}/{date.Day}/{id}";
            uriBuilder.Host = "localhost";
            uriBuilder.Port = 8080;
            var response = client.DeleteAsync(uriBuilder.Uri).Result;
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            return true;
        }

        public CalendarEvent GetEvent(DateTime date, int ID)
        {
            UriBuilder uriBuilder = new();
            uriBuilder.Path = $"/{date.Year}/{date.Month}/{date.Day}/{ID}";
            uriBuilder.Host = "localhost";
            uriBuilder.Port = 8080;
            var responseTask = client.GetAsync(uriBuilder.Uri);
            responseTask.Wait();
            var response = responseTask.Result;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var content = response.Content.ReadAsStringAsync().Result;
            CalendarEvent calendarEvent = JsonSerializer.Deserialize<CalendarEvent>(content);
            return calendarEvent;
        }

        public List<CalendarEventBasic> GetEvents(DateTime date)
        {
            UriBuilder uriBuilder = new();
            uriBuilder.Path = $"/{date.Year}/{date.Month}/{date.Day}/";
            uriBuilder.Host = "localhost";
            uriBuilder.Port = 8080;
            var responseTask = client.GetAsync(uriBuilder.Uri);
            responseTask.Wait();
            var response = responseTask.Result;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var content = response.Content.ReadAsStringAsync().Result;
            var events = JsonSerializer.Deserialize<List<CalendarEventBasic>>(content);
            return events;
        }

        public void SetClientDefaultUser()
        {
            user = defaultUser;
            var authenticationString = $"{defaultUser.name}:{defaultUser.password}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(authenticationString));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        }

        public bool SetClientUser(User user)
        {
            {
                this.user = user;
                var authenticationString = $"{user.name}:{user.password}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(authenticationString));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
            }

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Host = "localhost";
            uriBuilder.Port = 8080;
            uriBuilder.Path = "/Login";

            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Head;
            request.RequestUri = uriBuilder.Uri;

            var response = client.SendAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            {
                this.user = defaultUser;
                var authenticationString = $"{defaultUser.name}:{defaultUser.password}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(authenticationString));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
            }
            return false;

        }
    }
}
