using CalendarCommon;
using Microsoft.Win32;
using System;
using System.Collections;
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
        public bool GetEvent(DateTime date, int ID, out CalendarEvent calendarEvent);
        public bool GetEvents(DateTime dateBegin, out List<CalendarEventBasic> calendarEvents);
        public bool DeleteEvent(DateTime date, int ID);
    }
}
