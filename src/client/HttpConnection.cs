using CalendarCommon;
using System.Collections;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace CalendarClient
{
    /// <summary>
    /// Class that implements the IConnection interface to manage user and event data through HTTP requests.
    /// This class handles user authentication and interaction with a server using an HttpClient.
    /// </summary>
    public class HttpConnection : IConnection
    {
        // Default user for the client
        private User defaultUser;
        // Current user
        private User user;
        // HttpClient for comunication with the server
        private HttpClient client;

        /// <summary>
        /// Constructor that initializes the HttpConnection with a default user and configures the HTTP client.
        /// </summary>
        /// <param name="defaultUser">The default user to be used for authentication.</param>
        public HttpConnection(User defaultUser)
        {
            this.defaultUser = defaultUser;
            this.user = defaultUser;
            client = new HttpClient();
            var authenticationString = $"{defaultUser.name}:{defaultUser.password}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(authenticationString));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        }

        /// <summary>
        /// Creates a new user by sending a POST request to the server.
        /// </summary>
        /// <param name="user">The user to be created.</param>
        /// <returns>Returns true if the user is successfully created, false otherwise.</returns>
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

        /// <summary>
        /// Changes the user's details (either name or password) by sending a POST request to the server.
        /// Updates the authentication headers if successful.
        /// </summary>
        /// <param name="newUser">The updated user information.</param>
        /// <returns>Returns true if the user is successfully updated, false otherwise.</returns
        public bool ChangeUser(User newUser)
        {
            UriBuilder uriBuilder = new();
            // Sets the appropriate path based on whether the username or password is being changed.
            uriBuilder.Path = user.name == newUser.name ? "/User/Change/Password/" : "/User/Change/Name/";
            uriBuilder.Host = "localhost";
            uriBuilder.Port = 8080;

            var content = JsonContent.Create(newUser);
            var responseTask = client.PostAsync(uriBuilder.Uri, content);
            responseTask.Wait();
            var response = responseTask.Result;
            if (response.IsSuccessStatusCode)
            {
                // Updates the current user and the authentication headers
                user = newUser;
                var authenticationString = $"{user.name}:{user.password}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(authenticationString));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Saves a calendar event by sending a POST request to the server.
        /// Updates the event's ID based on the server's response.
        /// </summary>
        /// <param name="calendarEvent">The calendar event to be saved.</param>
        /// <returns>Returns true if the event is successfully saved, false otherwise.</returns>
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
            // Retrieves the event ID from the server's response
            var idTask = response.Content.ReadAsStringAsync();
            var id = idTask.Result;
            calendarEvent.id = int.Parse(id);
            return true;
        }

        /// <summary>
        /// Deletes a specific calendar event by sending a DELETE request to the server.
        /// </summary>
        /// <param name="date">The date of the event.</param>
        /// <param name="id">The ID of the event to be deleted.</param>
        /// <returns>Returns true if the event is successfully deleted, false otherwise.</returns>
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

        /// <summary>
        /// Retrieves a specific calendar event by sending a GET request to the server.
        /// </summary>
        /// <param name="date">The date of the event.</param>
        /// <param name="ID">The ID of the event to be retrieved.</param>
        /// <param name="calendarEvent">The retrieved calendar event (output parameter).</param>
        /// <returns>Returns true if the event is successfully retrieved, false otherwise.</returns>
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

        /// <summary>
        /// Retrieves calendar events.
        /// </summary>
        /// <returns>Returns an IEvents of CalendarEventBasic objects.</returns>
        public IEvents<CalendarEventBasic> GetEvents()
        {
            return new Events<CalendarEventBasic>(client);
        }

        /// <summary>
        /// Resets the client user to the default user and updates the authentication headers.
        /// </summary>
        public void SetClientDefaultUser()
        {
            user = defaultUser;
            var authenticationString = $"{defaultUser.name}:{defaultUser.password}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(authenticationString));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        }

        /// <summary>
        /// Sets the client user to a specified user and verifies the credentials by sending a HEAD request to the server.
        /// If unsuccessful, the user is reset to the default user.
        /// </summary>
        /// <param name="user">The user to be set for the session.</param>
        /// <returns>Returns true if the user is successfully set, false otherwise.</returns>
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
            // If login fails, reset the user to the default
            {
                this.user = defaultUser;
                var authenticationString = $"{defaultUser.name}:{defaultUser.password}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(authenticationString));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
            }
            return false;

        }
    }

    /// <summary>
    /// Static class that provides extension methods for the DateTime class.
    /// </summary>
    public static class DateTimeExtentions
    {
        /// <summary>
        /// Converts a DateTime object into a LowerDateBound structure.
        /// </summary>
        /// <param name="date">The DateTime object.</param>
        /// <returns>Returns a LowerDateBound object initialized with the provided date.</returns>
        public static LowerDateBound LowerDateBound(this DateTime date) => new LowerDateBound(date);

        /// <summary>
        /// Converts a DateTime object into an UpperDateBound structure.
        /// </summary>
        /// <param name="date">The DateTime object.</param>
        /// <returns>Returns an UpperDateBound object initialized with the provided date.</returns>
        public static UpperDateBound UpperDateBound(this DateTime date) => new UpperDateBound(date);
    }

    /// <summary>
    /// Class representing a collection of calendar events. Implements the IEvents interface to handle event filtering and retrieval.
    /// This class uses the HttpClient to interact with a server and retrieve events based on date bounds.
    /// </summary>
    /// <typeparam name="T">The type of events in the collection, which must be derived from CalendarEventBasic.</typeparam>
    public class Events<T> : IEvents<T> where T : CalendarEventBasic
    {
        LowerDateBound beginning;   // The lower date bound for filtering events
        UpperDateBound end;         // The upper date bound for filtering events
        HttpClient client;

        /// <summary>
        /// Constructor that initializes the Events collection with an HttpClient and default date bounds.
        /// The beginning date is set to the current date, and the end date is set to the maximum DateTime value.
        /// </summary>
        /// <param name="client">The HttpClient used to retrieve events from the server.</param>
        public Events(HttpClient client)
        {
            this.client = client;
            beginning = DateTime.Now.LowerDateBound();
            end = DateTime.MaxValue.UpperDateBound();
        }

        /// <summary>
        /// Constructor that initializes the Events collection with an HttpClient and specific date bounds.
        /// </summary>
        /// <param name="client">The HttpClient used to retrieve events from the server.</param>
        /// <param name="beginning">The lower bound for filtering events.</param>
        /// <param name="end">The upper bound for filtering events.</param>
        public Events(HttpClient client, LowerDateBound beginning, UpperDateBound end)
        {
            this.client = client;
            this.beginning = beginning;
            this.end = end;
        }

        /// <summary>
        /// Retrieves and enumerates through events in the specified date range.
        /// It iterates over events in time span up to 100 days.
        /// </summary>
        /// <returns>Returns an enumerator that iterates through the events.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            DateTime date = (DateTime)beginning;
            int counter = 0;
            while (date < (DateTime)end && counter < 100)
            {
                // Fetches events for a whole day at once
                List<T> events = new List<T>();
                if (GetEvents(date, out events))
                {
                    foreach (var calEvent in events)
                    {
                        yield return calEvent;
                    }
                }
                date = date.AddDays(1);
                counter++;
            }
        }

        /// <summary>
        /// Non-generic enumerator required by the IEnumerable interface.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Filters the event list based on a specified predicate. This adjusts the beginning or end date bounds for the event query.
        /// Handles only >= and < operators.
        /// </summary>
        /// <param name="expression">An expression that specifies the filter criteria.</param>
        /// <returns>Returns a new Events object with updated date bounds based on the filter criteria.</returns>
        public IEvents<T> Where(Expression<Predicate<T>> expression)
        {
            var lambda = expression.Body as BinaryExpression;
            var type = lambda?.NodeType;
            var name = (lambda?.Left as MemberExpression)?.Member.Name;
            var memberExpr = (lambda?.Right as UnaryExpression)?.Operand as MemberExpression;
            var constantExpr = memberExpr?.Expression as ConstantExpression;
            var field = memberExpr?.Member as FieldInfo;
            var date = field?.GetValue(constantExpr?.Value);

            // If the filter is for events greater than or equal to a specific date, adjust the lower date bound
            if (type == ExpressionType.GreaterThanOrEqual && name == "beginning")
            {
                if (date is DateTime lowerDate)
                {
                    beginning = lowerDate.LowerDateBound();
                }
            }
            // If the filter is for events less than a specific date, adjust the upper date bound
            else if (type == ExpressionType.LessThan && name == "beginning")
            {
                if (date is DateTime upperDate)
                {
                    end = upperDate.UpperDateBound();
                }
            }
            return new Events<T>(client, beginning, end);
        }

        /// <summary>
        /// Retrieves events for a specific date by sending a GET request to the server.
        /// </summary>
        /// <param name="date">The date of the events to retrieve.</param>
        /// <param name="calendarEvents">The list of events retrieved from the server (output parameter).</param>
        /// <returns>Returns true if events are successfully retrieved, false otherwise.</returns>
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

    /// <summary>
    /// Struct representing an upper bound for a date range.
    /// </summary>
    public struct UpperDateBound
    {
        private DateTime date { get; set; }

        /// <summary>
        /// Initializes the UpperDateBound with a specific date.
        /// </summary>
        /// <param name="date">The date to set as the upper bound.</param>
        public UpperDateBound(DateTime date) { this.date = date; }

        /// <summary>
        /// Converts an UpperDateBound object to a DateTime.
        /// </summary>
        /// <param name="date">The UpperDateBound object to convert.</param>
        public static explicit operator DateTime(UpperDateBound date) => date.date;
    }

    /// <summary>
    /// Struct representing a lower bound for a date range.
    /// </summary>
    public struct LowerDateBound
    {
        private DateTime date;

        /// <summary>
        /// Initializes the LowerDateBound with a specific date.
        /// </summary>
        /// <param name="date">The date to set as the lower bound.</param>
        public LowerDateBound(DateTime date)
        {
            this.date = date;
        }

        /// <summary>
        /// Converts a LowerDateBound object to a DateTime.
        /// </summary>
        /// <param name="date">The LowerDateBound object to convert.</param>
        public static explicit operator DateTime(LowerDateBound date) => date.date;
    }
}
