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

        public void DeleteEvent(DateTime dateTime, int id)
        {
            throw new NotImplementedException();
        }

        public List<CalendarEvent> GetEvents(Expression e)
        {
            throw new NotImplementedException();
        }

        public void SaveEvent(CalendarEvent e)
        {
            throw new NotImplementedException();
        }
    }
}
