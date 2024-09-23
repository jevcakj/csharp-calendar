using CalendarCommon;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public IEventList<CalendarEventBasic> GetEvents();
        public bool DeleteEvent(DateTime date, int ID);
    }
    public interface IEventList<T> : IEnumerable<T> where T : CalendarEventBasic
    {
        public IEventList<T> Where(Expression<Predicate<T>> expression);
    }
}
