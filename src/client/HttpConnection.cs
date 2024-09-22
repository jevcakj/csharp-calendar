using CalendarCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static CalendarClient.HttpConnection;

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

        public bool GetEvent(DateTime date, int ID, out CalendarEvent calendarEvent)
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
                calendarEvent = new();
                return false;
            }
            var content = response.Content.ReadAsStringAsync().Result;
            var calendarEventNull = JsonSerializer.Deserialize<CalendarEvent>(content);
            if( calendarEventNull is null)
            {
                calendarEvent = new();
                return false;
            }
            calendarEvent = calendarEventNull;
            return true;
        }

        public bool GetEvents(DateTime date, out List<CalendarEventBasic> calendarEvents)
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
                calendarEvents = new List<CalendarEventBasic>();
                return false;
            }
            var content = response.Content.ReadAsStringAsync().Result;
            var calendarEventsNull = JsonSerializer.Deserialize<List<CalendarEventBasic>>(content);
            if(calendarEventsNull is null)
            {
                calendarEvents = new List<CalendarEventBasic>();
                return false;
            }
            calendarEvents = calendarEventsNull;
            return true;
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

        public Events<CalendarEventBasic> Basics()
        {
            return new Events<CalendarEventBasic>(client);
        }

        public class Events<T> : IEnumerable<T> where T : CalendarEventBasic
        {
            LowerDateBound beginning;
            UpperDateBound end;
            HttpClient client;
            public Events(HttpClient client)
            {
                this.client = client;
                beginning = DateTime.Now.LowerDateBound();
                end = DateTime.Now.AddDays(7).UpperDateBound();
            }
            public Events(HttpClient client, LowerDateBound date)
            {
                this.client = client;
                beginning = date;
                end = ((DateTime)date).AddDays(7).UpperDateBound();
            }
            public Events(HttpClient client, UpperDateBound date)
            {
                this.client = client;
                end = date;
                beginning = ((DateTime)date).AddDays(-7).LowerDateBound();
            }
            public Events(HttpClient client, LowerDateBound beginning, UpperDateBound end)
            {
                this.client = client;
                this.beginning = beginning;
                this.end = end;
            }

            public IEnumerator<T> GetEnumerator()
            {
                DateTime date = (DateTime)beginning;
                while(date <= (DateTime)end)
                {
                    List<T> events = new List<T>();
                    if (GetEvents(date, out events))
                    {
                        foreach(var calEvent in events)
                        {
                            yield return calEvent;
                        }
                    }
                    date = date.AddDays(1);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            public Events<T> Where(Expression<Predicate<T>> expression)
            {
                var lambda = expression.Body as BinaryExpression;
                var type = lambda?.NodeType;
                var name = (lambda?.Left as MemberExpression)?.Member.Name;
                var memberExpr = (lambda?.Right as UnaryExpression)?.Operand as MemberExpression;
                var constantExpr = memberExpr?.Expression as ConstantExpression;
                var field = memberExpr?.Member as FieldInfo;
                var date = field?.GetValue(constantExpr?.Value);

                if (type == ExpressionType.GreaterThanOrEqual && name == "beginning")
                {
                    if(date is DateTime lowerDate)
                    {
                        beginning = lowerDate.LowerDateBound();
                    }
                }
                else if(type == ExpressionType.LessThanOrEqual && name == "beginning")
                {
                    if(date is DateTime upperDate)
                    {
                        end = upperDate.UpperDateBound();
                    }
                }
                return new(client, beginning, end);
            }
            private bool GetEvents(DateTime date, out List<T> calendarEvents)
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
                    calendarEvents = new List<T>();
                    return false;
                }
                var content = response.Content.ReadAsStringAsync().Result;
                var calendarEventsNull = JsonSerializer.Deserialize<List<T>>(content);
                if (calendarEventsNull is null)
                {
                    calendarEvents = new List<T>();
                    return false;
                }
                calendarEvents = calendarEventsNull;
                return true;
            }
        }
        public struct UpperDateBound
        {
            private DateTime date {  get; set; }
            public UpperDateBound(DateTime date) { this.date = date; }
            public static explicit operator DateTime(UpperDateBound date) => date.date;
        }
        public struct LowerDateBound
        {
            private DateTime date;
            public LowerDateBound(DateTime date)
            {
                this.date = date;
            }
            public static explicit operator DateTime(LowerDateBound date) => date.date;
        }
        
    }
    public static class DateTimeExtentions
    {
        public static LowerDateBound LowerDateBound(this DateTime date) => new LowerDateBound(date);
        public static UpperDateBound UpperDateBound(this DateTime date) => new UpperDateBound(date);
    }
}
