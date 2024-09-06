using CalendarCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarClient
{
    public class HttpConnection : IConnection
    {
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
            throw new NotImplementedException();
        }

        public bool SetClientUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
