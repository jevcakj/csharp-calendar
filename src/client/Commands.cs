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
        public string CommandString { get; set; }

        void Execute();
        bool CheckArguments();
        ICalendarCommand Copy();
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
            //connection.CreateUser(user);
        }
        public bool CheckArguments() => true;
        public ICalendarCommand Copy()
        {
            return new CreateUserCommand(ui, connection)
            {
                CommandString = CommandString
            };
        }
    }


    public class LoginUserCommand : ICalendarCommand
    {
        public string CommandString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CheckArguments()
        {
            throw new NotImplementedException();
        }

        public ICalendarCommand Copy()
        {
            throw new NotImplementedException();
        }

        public void Execute() { }
    }

    public class LogoutUserCommand : ICalendarCommand
    {
        public string CommandString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CheckArguments()
        {
            throw new NotImplementedException();
        }

        public ICalendarCommand Copy()
        {
            throw new NotImplementedException();
        }

        public void Execute() { }
    }

    public class ChangeUserNameCommand : ICalendarCommand
    {
        public string CommandString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CheckArguments()
        {
            throw new NotImplementedException();
        }

        public ICalendarCommand Copy()
        {
            throw new NotImplementedException();
        }

        public void Execute() { }
    }

    public class ChangeUserPasswordCommand : ICalendarCommand
    {
        public string CommandString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CheckArguments()
        {
            throw new NotImplementedException();
        }

        public ICalendarCommand Copy()
        {
            throw new NotImplementedException();
        }

        public void Execute() { }
    }

    public class AddEventCommand : ICalendarCommand
    {
        public string CommandString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CheckArguments()
        {
            throw new NotImplementedException();
        }

        public ICalendarCommand Copy()
        {
            throw new NotImplementedException();
        }

        public void Execute() { }
    }

    public class DeleteEventCommand : ICalendarCommand
    {
        public string CommandString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CheckArguments()
        {
            throw new NotImplementedException();
        }

        public ICalendarCommand Copy()
        {
            throw new NotImplementedException();
        }

        public void Execute() { }
    }

    public class EditEventCommand : ICalendarCommand
    {
        public string CommandString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CheckArguments()
        {
            throw new NotImplementedException();
        }

        public ICalendarCommand Copy()
        {
            throw new NotImplementedException();
        }

        public void Execute() { }
    }

    public class ListEventsCommand : ICalendarCommand
    {
        public string CommandString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CheckArguments()
        {
            throw new NotImplementedException();
        }

        public ICalendarCommand Copy()
        {
            throw new NotImplementedException();
        }

        public void Execute() { }
    }

    public class ShowEventCommand : ICalendarCommand
    {
        public string CommandString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CheckArguments()
        {
            throw new NotImplementedException();
        }

        public ICalendarCommand Copy()
        {
            throw new NotImplementedException();
        }

        public void Execute() { }
    }

    public class NextCommand : ICalendarCommand
    {
        public string CommandString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CheckArguments()
        {
            throw new NotImplementedException();
        }

        public ICalendarCommand Copy()
        {
            throw new NotImplementedException();
        }

        public void Execute() { }
    }

    public class PreviousCommand : ICalendarCommand
    {
        public string CommandString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CheckArguments()
        {
            throw new NotImplementedException();
        }

        public ICalendarCommand Copy()
        {
            throw new NotImplementedException();
        }

        public void Execute() { }
    }

    public class CurrentCommand : ICalendarCommand
    {
        public string CommandString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CheckArguments()
        {
            throw new NotImplementedException();
        }

        public ICalendarCommand Copy()
        {
            throw new NotImplementedException();
        }

        public void Execute() { }
    }

    public class ViewCommand : ICalendarCommand
    {
        public string CommandString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CheckArguments()
        {
            throw new NotImplementedException();
        }

        public ICalendarCommand Copy()
        {
            throw new NotImplementedException();
        }

        public void Execute() { }
    }
}
