using CalendarCommon;
using System.Text.Json;

namespace CalendarServer
{
    /// <summary>
    /// Class that implements the IData interface to handle user and event data storage in the file system.
    /// This class manages users and events by reading and writing to files in a directory structure.
    /// </summary>
    public class FileDataStorage : IData
    {
        /// <summary>
        /// Gets the root directory path where user and event data is stored.
        /// </summary>
        public string rootDirectory { get; init; }

        /// <summary>
        /// Initializes the FileDataStorage with a specified root directory path.
        /// Ensures that the directory exists and is valid.
        /// If no directory is specified, it initializes with Data/ directory placed in src/.
        /// </summary>
        /// <param name="path">The relative or absolute path to the root data directory. Defaults to a relative path.</param>
        public FileDataStorage(string path = "..\\..\\..\\..\\Data")
        {
            if(!Path.IsPathRooted(path) )
            {
                path = Path.GetFullPath(Path.Join(Directory.GetCurrentDirectory(), path));
            }
            
            if (!Directory.Exists(path))
            {
                throw new Exception("Directory does not exists.");
            }
            rootDirectory = path;
        }

        /// <summary>
        /// Creates a new user by creating a directory for the user and saving their information in a file.
        /// </summary>
        /// <param name="user">The user object containing the new user's information.</param>
        /// <returns>Returns true if the user is created successfully, otherwise false if the user already exists.</returns>
        public bool CreateUser(User user)
        {
            var path = Path.Join(rootDirectory, user.name);
            if (Path.Exists(path))
            {
                return false;
            }
            Directory.CreateDirectory(path);
            File.WriteAllText(Path.Join(path, "userInfo"), JsonSerializer.Serialize(user));
            File.WriteAllText(Path.Join(path, "IDCounter"), "0");
            return true;
        }

        /// <summary>
        /// Updates the user's username by renaming their directory and updating their information.
        /// </summary>
        /// <param name="oldUser">The existing user whose name is being changed.</param>
        /// <param name="newUser">The user object containing the updated username.</param>
        /// <returns>Returns true if the username is updated successfully, otherwise false if the new username already exists.</returns>
        public bool UpdateUserName(User oldUser, User newUser)
        {
            Console.WriteLine("update username");
            var oldPath = Path.Join(rootDirectory, oldUser.name);
            var newPath = Path.Join(rootDirectory, newUser.name);
            if (Path.Exists(newPath))
            {
                return false;
            }
            Directory.Move(oldPath, newPath);
            File.WriteAllText(Path.Join(newPath, "userInfo"), JsonSerializer.Serialize(newUser));
            return true;
        }

        /// <summary>
        /// Updates the user's password by modifying their user information file.
        /// </summary>
        /// <param name="oldUser">The existing user whose password is being changed.</param>
        /// <param name="newUser">The user object containing the updated password.</param>
        /// <returns>Returns true after the password is updated successfully.</returns>
        public bool UpdateUserPassword(User oldUser, User newUser)
        {
            Console.WriteLine("update password");
            oldUser.password = newUser.password;
            var path = Path.Join(rootDirectory , oldUser.name, "userInfo");
            File.WriteAllText(path, JsonSerializer.Serialize(oldUser));
            return true;
        }

        /// <summary>
        /// Authenticates the user by verifying their credentials against the stored user data.
        /// </summary>
        /// <param name="user">The user object containing the credentials to authenticate.</param>
        /// <returns>Returns true if the user is authenticated successfully, otherwise false.</returns>
        public bool AuthenticateUser(User user)
        {
            var path = Path.Join(rootDirectory, user.name);
            if (!Path.Exists(path))
            {
                return false;
            }
            path = Path.Join(path, "userInfo");
            var userCheck = JsonSerializer.Deserialize<User>(File.ReadAllText(path));
            return user.password == userCheck.password;
        }

        /// <summary>
        /// Deletes a specific calendar event for a user by deleting the corresponding event file.
        /// </summary>
        /// <param name="dateTime">The date of the event.</param>
        /// <param name="id">The ID of the event to be deleted.</param>
        /// <param name="user">The user who owns the event.</param>
        public void DeleteEvent(DateTime dateTime, int id, User user)
        {
            var path = Path.Join(rootDirectory, user.name, $"{dateTime.Year}", $"{dateTime.Month}", $"{dateTime.Day}", $"{id}");
            if (Path.Exists(path))
            {
                File.Delete(path);
                Console.WriteLine($"Event deleted: {path}");
            }
        }

        /// <summary>
        /// Retrieves all calendar events for a user on a specific date.
        /// </summary>
        /// <param name="dateTime">The date of the events to retrieve.</param>
        /// <param name="user">The user whose events are being retrieved.</param>
        /// <param name="calendarEvents">The list of calendar events (output parameter).</param>
        /// <returns>Returns true if events are retrieved successfully, otherwise false.</returns>
        public bool GetEvents(DateTime dateTime, User user, out List<CalendarEventBasic> calendarEvents)
        {
            var path = Path.Join(rootDirectory, user.name, $"{dateTime.Year}", $"{dateTime.Month}", $"{dateTime.Day}");
            Console.WriteLine($"Get events: {path}");
            calendarEvents = new List<CalendarEventBasic>();
            if (!Path.Exists(path))
            {
                return false;
            }
            foreach(var eventPath in Directory.GetFiles(path))
            {
                var calEvent = JsonSerializer.Deserialize<CalendarEventBasic>(File.ReadAllText(eventPath));
                if (calEvent != null)
                {
                    calendarEvents.Add(calEvent);
                }
            }
            return true;
        }

        /// <summary>
        /// Retrieves a specific calendar event for a user based on its date and ID.
        /// </summary>
        /// <param name="dateTime">The date of the event.</param>
        /// <param name="id">The ID of the event.</param>
        /// <param name="user">The user who owns the event.</param>
        /// <param name="calendarEvent">The calendar event object (output parameter).</param>
        /// <returns>Returns true if the event is retrieved successfully, otherwise false.</returns>
        public bool GetEvent(DateTime dateTime, int id, User user, out CalendarEvent calendarEvent)
        {
            var path = Path.Join(rootDirectory, user.name, $"{dateTime.Year}", $"{dateTime.Month}", $"{dateTime.Day}", $"{id}");
            Console.WriteLine($"Get event: {path}");
            if (!Path.Exists(path))
            {
                calendarEvent = null;
                return false;
            }
            calendarEvent = JsonSerializer.Deserialize<CalendarEvent>(File.ReadAllText(path));
            if (calendarEvent == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Saves a calendar event for a user and returns the event's ID.
        /// If the event has no ID, a new ID is generated.
        /// </summary>
        /// <param name="e">The calendar event to be saved.</param>
        /// <param name="user">The user who owns the event.</param>
        /// <returns>Returns the event's ID after saving.</returns>
        public int SaveEvent(CalendarEvent e, User user)
        {
            var path = Path.Join(rootDirectory, user.name);
            int id = e.id ?? GetNewEventId(user);
            e.id = id;
            DateTime dateTime = (DateTime)e.beginning;
            path = Path.Join(path, $"{dateTime.Year}", $"{dateTime.Month}", $"{dateTime.Day}");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Join(path, $"{id}");
            File.WriteAllText(path, JsonSerializer.Serialize(e));
            Console.WriteLine($"Event saved: {path}");
            return id;
        }

        /// <summary>
        /// Generates a new unique ID for a user's events by incrementing the event counter.
        /// </summary>
        /// <param name="user">The user for whom the event ID is being generated.</param>
        /// <returns>Returns a new unique event ID.</returns>
        private int GetNewEventId(User user)
        {
            var path = Path.Join(rootDirectory, user.name, "IDCounter");
            var id = int.Parse(File.ReadAllText(path));
            File.WriteAllText(path, $"{++id}");
            return id;
        }
    }
}
