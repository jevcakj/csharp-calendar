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
        public void CreateUser(User user)
        {
            var path = Path.Join(rootDirectory, user.Name);
            Directory.CreateDirectory(path);
            File.WriteAllText(Path.Join(path, "userInfo"), JsonSerializer.Serialize(user));
        }
        public bool AuthenticateUser(User user)
        {
            return true;
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
