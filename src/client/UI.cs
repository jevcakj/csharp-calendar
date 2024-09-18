using CalendarCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarClient
{
    public interface IUserInterface
    {
        public string UserName { get; set; }
        public ICalendarCommand GetInput();
        public User LoginUser();
        public void LogoutUser();
        public User CreateUser();
        public User ChangeUserName();
        public User ChangeUserPassword();
        public CalendarEvent AddEvent();
        public bool DeleteEvent(CalendarEventBasic calendarEvent);
        public CalendarEvent EditEvent(CalendarEvent calendarEvent);
        public void ShowEvent(CalendarEvent calendarEvent);
        public void ListEvents( List<CalendarEventBasic> events);
        public void Help();
        public void ShowMessage(string message);
    }
}
