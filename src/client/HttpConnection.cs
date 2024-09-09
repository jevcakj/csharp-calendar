using CalendarCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CalendarClient
{
    public class HttpConnection : IConnection
    {
        private User defaultUser;
        private HttpClient client;
        public HttpConnection(User defaultUser)
        {
            this.defaultUser = defaultUser;
            client = new HttpClient();
            var authenticationString = $"{defaultUser.name}:{defaultUser.password}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(authenticationString));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        }
        public void ChangeUser(User oldUser, User newUser)
        {
            throw new NotImplementedException();
        }

        public bool CreateUser(User user)
        {
            throw new NotImplementedException();
        }

        public void DeleteEvent(DateTime date, int ID, User user)
        {
            throw new NotImplementedException();
        }

        public void GetEvent(DateTime date, int ID, User user)
        {
            throw new NotImplementedException();
        }

        public void GetEvents(DateTime dateBegin, DateTime dateEnd, User user)
        {
            throw new NotImplementedException();
        }

        public void SaveEvent(CalendarEvent calendarEvent, User user)
        {
            throw new NotImplementedException();
        }

        public void SetClientDefaultUser()
        {
            var authenticationString = $"{defaultUser.name}:{defaultUser.password}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(authenticationString));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        }

        public bool SetClientUser(User user)
        {
            {
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
                var authenticationString = $"{defaultUser.name}:{defaultUser.password}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(authenticationString));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
            }
            return false;

        }
    }
}
