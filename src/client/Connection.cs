using CalendarCommon;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarClient
{
    public interface IConnection
    {
        public bool SetClientUser(User user);
        public void SetClientDefaultUser();
        public bool CreateUser(User user);
        public bool ChangeUser(User newUser);
        public bool SaveEvent(CalendarEvent calendarEvent);
        public void GetEvent(DateTime date, int ID, User user);
        public void GetEvents(DateTime dateBegin, DateTime dateEnd, User user);
        public void DeleteEvent(DateTime date, int ID, User user);
    }
}
