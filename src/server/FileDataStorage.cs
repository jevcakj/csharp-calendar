using CalendarCommon;
using CalendarServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CalendarServer
{
    public class FileDataStorage : IData
    {
        public string rootDirectory { get; init; }
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
        public bool UpdateUserPassword(User oldUser, User newUser)
        {
            Console.WriteLine("update password");
            oldUser.password = newUser.password;
            var path = Path.Join(rootDirectory , oldUser.name, "userInfo");
            File.WriteAllText(path, JsonSerializer.Serialize(oldUser));
            return true;
        }
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

        public void DeleteEvent(DateTime dateTime, int id, User user)
        {
            var path = Path.Join(rootDirectory, user.name, $"{dateTime.Year}", $"{dateTime.Month}", $"{dateTime.Day}", $"{id}");
            if (Path.Exists(path))
            {
                File.Delete(path);
                Console.WriteLine($"Event deleted: {path}");
            }
        }

        public bool GetEvents(DateTime dateTime, User user, out List<CalendarEvent> calendarEvents)
        {
            var path = Path.Join(rootDirectory, user.name, $"{dateTime.Year}", $"{dateTime.Month}", $"{dateTime.Day}");
            Console.WriteLine($"Get events: {path}");
            calendarEvents = new List<CalendarEvent>();
            if (!Path.Exists(path))
            {
                return false;
            }
            foreach(var eventPath in Directory.GetFiles(path))
            {
                var calEvent = JsonSerializer.Deserialize<CalendarEvent>(File.ReadAllText(eventPath));
                if (calEvent != null)
                {
                    calendarEvents.Add(calEvent);
                }
            }
            return true;
        }

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

        public int SaveEvent(CalendarEvent e, User user)
        {
            var path = Path.Join(rootDirectory, user.name);
            int id = e.id ?? GetNewEventId(user);
            e.id = id;
            DateTime dateTime = (DateTime)e.dateTime;
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

        private int GetNewEventId(User user)
        {
            var path = Path.Join(rootDirectory, user.name, "IDCounter");
            var id = int.Parse(File.ReadAllText(path));
            File.WriteAllText(path, $"{++id}");
            return id;
        }
    }
}
