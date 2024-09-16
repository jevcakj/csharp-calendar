using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using CalendarCommon;

namespace CalendarServer
{
    public interface IData
    {
        public bool CreateUser(User user);
        public bool UpdateUserName(User oldUser, User newUser);
        public bool UpdateUserPassword(User oldUser, User newUser);
        public bool AuthenticateUser(User user);
        public int SaveEvent(CalendarEvent e, User user);
        public void DeleteEvent(DateTime dateTime, int id, User user);
        public bool GetEvents(DateTime dateTime, User user, out List<CalendarEventBasic> calendarEvents);
        public bool GetEvent(DateTime dateTime, int id, User user, out CalendarEvent calendarEvent);
    }
}
