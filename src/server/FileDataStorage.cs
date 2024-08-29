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
        private string rootDirectory;
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
            Console.WriteLine($"Data dir: {rootDirectory}");

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

        public List<CalendarEvent> GetEvents(Expression e, User user)
        {
            throw new NotImplementedException();
        }

        public int SaveEvent(CalendarEvent e, User user)
        {
            var path = Path.Join(rootDirectory, user.name);
            int id = GetNewEventId(user);
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
