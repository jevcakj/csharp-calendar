using CalendarCommon;
using CalendarServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CalendarServer
{
    public class FileDataStorage : IData
    {
        public void CreateUser(User user)
        {
            throw new NotImplementedException();
        }

        public void DeleteEvent(DateTime dateTime, int id)
        {
            throw new NotImplementedException();
        }

        public List<CalendarEvent> GetEvents(Expression e)
        {
            throw new NotImplementedException();
        }

        public User GetUser(string name)
        {
            throw new NotImplementedException();
        }

        public void SaveEvent(CalendarEvent e)
        {
            throw new NotImplementedException();
        }
    }
}
