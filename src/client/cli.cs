using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalendarCommon;

namespace CalendarClient
{
    public class CommandLineInterface : IUserInterface
    {
        private Dictionary<string, ICalendarCommand> commands;
        public CommandLineInterface(Dictionary<string, ICalendarCommand> commands)
        {
            this.commands = commands;
        }
        public void AddEvent()
        {
            throw new NotImplementedException();
        }

        public User ChangeUserName()
        {
            throw new NotImplementedException();
        }

        public User ChangeUserPassword()
        {
            throw new NotImplementedException();
        }

        public User CreateUser()
        {
            throw new NotImplementedException();
        }

        public void DeleteEvent()
        {
            throw new NotImplementedException();
        }

        public void EditEvent()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetInput()
        {
            throw new NotImplementedException();
        }

        public void ListEvents()
        {
            throw new NotImplementedException();
        }

        public User LoginUser()
        {
            throw new NotImplementedException();
        }

        public void LogoutUser()
        {
            throw new NotImplementedException();
        }

        public void ShowEvent()
        {
            throw new NotImplementedException();
        }

        public void ShowMessage()
        {
            throw new NotImplementedException();
        }
    }
}
