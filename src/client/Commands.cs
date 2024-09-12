using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CalendarCommon;

namespace CalendarClient
{
    //TODO exit command
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
            CommandString = "";
        }
        public void Execute()
        {
            User user = ui.CreateUser();
            if (connection.CreateUser(user))
                return;
            ui.ShowMessage("Unable to create user");
        }
        public bool CheckArguments() => CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries ).Length == 1;
        public ICalendarCommand Copy()
        {
            return new CreateUserCommand(ui, connection)
            {
                CommandString = CommandString
            };
        }
    }

    public class ChangeUserNameCommand : ICalendarCommand
    {
        private IUserInterface ui;
        private IConnection connection;
        private Client client;
        public string CommandString { get; set; }
        public ChangeUserNameCommand(IUserInterface ui, IConnection connection, Client client)
        {
            this.ui = ui;
            this.connection = connection;
            CommandString = "";
            this.client = client;
        }
        public bool CheckArguments() => CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Length == 1;

        public ICalendarCommand Copy() => new ChangeUserNameCommand(ui, connection, client);

        public void Execute()
        {
            User user = ui.ChangeUserName();
            user.password = client.user.password;
            if (connection.ChangeUser(user))
            {
                client.user = user;
                ui.UserName = user.name;
            }
            else
            {
                ui.ShowMessage("Username is already taken. Choose different.");
            }
        }
    }

    public class ChangeUserPasswordCommand : ICalendarCommand
    {
        private IUserInterface ui;
        private IConnection connection;
        private Client client;
        public string CommandString { get; set; }
        public ChangeUserPasswordCommand(IUserInterface ui, IConnection connection, Client client)
        {
            this.ui = ui;
            this.connection = connection;
            CommandString = "";
            this.client = client;
        }
        public bool CheckArguments() => CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Length == 1;

        public ICalendarCommand Copy() => new ChangeUserPasswordCommand(ui, connection, client);

        public void Execute()
        {
            User user = ui.ChangeUserPassword();
            user.name = client.user.name;
            if (connection.ChangeUser(user))
            {
                client.user = user;
            }
            else
            {
                ui.ShowMessage("Unable to change password.");
            }
        }
    }

    public class LoginUserCommand : ICalendarCommand
    {
        private IUserInterface ui;
        private IConnection connection;
        private Client client;
        public string CommandString { get; set; }

        public LoginUserCommand(IUserInterface ui, IConnection connection, Client client)
        {
            this.ui = ui;
            this.connection = connection;
            CommandString = "";
            this.client = client;
        }

        public bool CheckArguments() => CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Length == 1;

        public ICalendarCommand Copy() => new LoginUserCommand(ui, connection, client);

        public void Execute()
        {
            User user = ui.LoginUser();
            if (connection.SetClientUser(user))
            {
                client.user = user;
                ui.UserName = user.name;
            }
            else
            {
                connection.SetClientDefaultUser();
                ui.ShowMessage("Invalid username or password.");
            }
        }
    }

    public class LogoutUserCommand : ICalendarCommand
    {
        private IUserInterface ui;
        private IConnection connection;
        public string CommandString { get; set; }
        public LogoutUserCommand(IUserInterface ui, IConnection connection)
        {
            this.ui= ui;
            this.connection = connection;
            CommandString = "";
        }
        public bool CheckArguments() => CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Length == 1;

        public ICalendarCommand Copy() => new LogoutUserCommand(ui, connection);

        public void Execute()
        {
            ui.LogoutUser();
            connection.SetClientDefaultUser();
        }
    }

    public class AddEventCommand : ICalendarCommand
    {
        private IUserInterface ui;
        private IConnection connection;
        public string CommandString { get; set; }
        public AddEventCommand(IUserInterface ui, IConnection connection)
        {
            this.ui= ui;
            this.connection= connection;
            CommandString = "";
        }
        public bool CheckArguments()
        {
            //throw new NotImplementedException();
            return true;
        }

        public ICalendarCommand Copy() => new AddEventCommand(ui, connection);

        public void Execute() { }
    }

    public class DeleteEventCommand : ICalendarCommand
    {
        private IUserInterface ui;
        private IConnection connection;
        public string CommandString { get; set; }
        public DeleteEventCommand(IUserInterface ui, IConnection connection)
        {
            this.ui= ui;
            this.connection= connection;
            CommandString = "";
        }
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
        private IUserInterface ui;
        private IConnection connection;
        public string CommandString { get; set; }
        public EditEventCommand(IUserInterface ui, IConnection connection)
        {
            this.ui= ui;
            this.connection= connection;
            CommandString = "";
        }
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
        private IUserInterface ui;
        private IConnection connection;
        public string CommandString { get; set; }
        public ListEventsCommand(IUserInterface ui, IConnection connection)
        {
            this.ui= ui;
            this.connection= connection;
            CommandString = "";
        }
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
        private IUserInterface ui;
        private IConnection connection;
        public string CommandString { get; set; }
        public ShowEventCommand(IUserInterface ui, IConnection connection)
        {
            this.ui= ui;
            this.connection= connection;
            CommandString = "";
        }
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
        private IUserInterface ui;
        private IConnection connection;
        public string CommandString { get; set; }
        public NextCommand(IUserInterface ui, IConnection connection)
        {
            this.ui= ui;
            this.connection= connection;
            CommandString = "";
        }
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
        private IUserInterface ui;
        private IConnection connection;
        public string CommandString { get; set; }
        public PreviousCommand(IUserInterface ui, IConnection connection)
        {
            this.ui= ui;
            this.connection= connection;
            CommandString = "";
        }
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
        private IUserInterface ui;
        private IConnection connection;
        public string CommandString { get; set; }
        public CurrentCommand(IUserInterface ui, IConnection connection)
        {
            this.ui= ui;
            this.connection= connection;
            CommandString = "";
        }
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
        private IUserInterface ui;
        private IConnection connection;
        public string CommandString { get; set; }
        public ViewCommand(IUserInterface ui, IConnection connection)
        {
            this.ui = ui;
            this.connection = connection;
            CommandString = "";
        }

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

    public class ExitCommand : ICalendarCommand
    {
        public string CommandString { get; set; }

        public ExitCommand()
        {
            CommandString = "exit";
        }

        public bool CheckArguments() => true;

        public ICalendarCommand Copy() => new ExitCommand();

        public void Execute() { }
    }
}
