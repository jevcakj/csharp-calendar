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
        public CalendarEvent GetEvent(DateTime date, int ID);
        public List<CalendarEvent> GetEvents(DateTime dateBegin);
        public bool DeleteEvent(DateTime date, int ID);
    }
}
