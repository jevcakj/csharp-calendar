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
        public User User { get; set; }
        public ICalendarCommand GetInput();
        public User LoginUser();
        public void LogoutUser();
        public User CreateUser();
        public User ChangeUserName();
        public User ChangeUserPassword();
        public void AddEvent();
        public void DeleteEvent();
        public void EditEvent();
        public void ShowEvent();
        public void ListEvents();
        public void ShowMessage(string message);
    }
}
