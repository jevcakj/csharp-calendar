using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CalendarCommon;

namespace CalendarClient
{
    public interface ICalendarCommand
    {
        void Execute();
    }

    public class CreateUserCommand : ICalendarCommand
    {
        private IUserInterface ui;
        private IConnection connection;
        public string CommandString { get; set; }   
        public CreateUserCommand(IUserInterface ui, IConnection connection)
        {
            this.ui = ui;
            this.connection = connection;
        }
        public void Execute()
        {
            User user = ui.CreateUser();
            connection.CreateUser(user);
        }
    }

    public class LoginUserCommand : ICalendarCommand
    {
        public void Execute() { }
    }

    public class LogoutUserCommand : ICalendarCommand
    {
        public void Execute() { }
    }

    public class ChangeUserNameCommand : ICalendarCommand
    {
        public void Execute() { }
    }

    public class ChangeUserPasswordCommand : ICalendarCommand
    {
        public void Execute() { }
    }

    public class AddEventCommand : ICalendarCommand
    {
        public void Execute() { }
    }

    public class DeleteEventCommand : ICalendarCommand
    {
        public void Execute() { }
    }

    public class EditEventCommand : ICalendarCommand
    {
        public void Execute() { }
    }

    public class ListEventsCommand : ICalendarCommand
    {
        public void Execute() { }
    }

    public class ShowEventCommand : ICalendarCommand
    {
        public void Execute() { }
    }

    public class NextCommand : ICalendarCommand
    {
        public void Execute() { }
    }

    public class PreviousCommand : ICalendarCommand
    {
        public void Execute() { }
    }

    public class CurrentCommand : ICalendarCommand
    {
        public void Execute() { }
    }

    public class ViewCommand : ICalendarCommand
    {
        public void Execute() { }
    }
}
