﻿using System;
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
        public bool CheckArguments() => CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Length == 1;

        public ICalendarCommand Copy() => new AddEventCommand(ui, connection);

        public void Execute()
        {
            CalendarEvent calendarEvent = ui.AddEvent();
            if(calendarEvent is null)
            {
                return;
            }
            if (!connection.SaveEvent(calendarEvent))
            {
                ui.ShowMessage("Adding event was not successfull. Try it again.");
            }
        }
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

    //TODO
    #region
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
        private Client client;
        public string CommandString { get; set; }
        private List<CalendarEvent> events;
        public ListEventsCommand(IUserInterface ui, IConnection connection, Client client)
        {
            this.ui = ui;
            this.connection = connection;
            this.client = client;
            CommandString = "";
            events = new List<CalendarEvent>();
        }
        public bool CheckArguments()
        {
            string[] args = CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if(args.Length != 1 && args.Length != 2)
            {
                return false;
            }
            DateTime date;
            if(args.Length == 2 && !DateTime.TryParse(args[1], out date))
            {
                return false;
            }
            return true;
        }

        public ICalendarCommand Copy() => new ListEventsCommand(ui, connection, client);

        public void Execute()
        {
            string[] args = CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (args.Length == 2)
            {
                DateTime shownDate = DateTime.Parse(args[1]);
                client.shownDate = shownDate;
            }
            DateTime date = client.BeginningDate();
            int numberOfDays = client.NumberOfDays();
            if(numberOfDays > 0)
            {
                for (int i = 0; i < numberOfDays; i++)
                {
                    List<CalendarEvent> events = connection.GetEvents(date.AddDays(i));
                    if(events is not null)
                    {
                        this.events.AddRange(events);
                    }
                }
            }
            else
            {
                for(;events.Count <= 10; date = date.AddDays(1))
                {
                    List<CalendarEvent> events = connection.GetEvents(date);
                    this.events.AddRange(events);
                }
                if(events.Count > 10)
                {
                    events.RemoveRange(10, events.Count - 10);
                }
            }
            ui.ListEvents(events);
        }
        public List<CalendarEvent> GetEvents() => events;
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
        private Client client;
        public string CommandString { get; set; }
        public ViewCommand(IUserInterface ui, IConnection connection, Client client)
        {
            this.ui = ui;
            this.connection = connection;
            this.client = client;
            CommandString = "";
        }

        public bool CheckArguments()
        {
            string[] args = CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if(args.Length != 2)
            {
                return false;
            }
            if (args[1] != "week" && args[1] != "month" && args[1] != "upcomming")
            {
                return false;
            }
            return true;
        }

        public ICalendarCommand Copy() => new ViewCommand(ui, connection, client);

        public void Execute()
        {
            string[] args = CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            switch (args[1])
            {
                case "month":
                    client.SetView(ViewSpan.Month);
                    break;
                case "upcomming":
                    client.SetView(ViewSpan.Upcomming);
                    break;
                case "week":
                default:
                    client.SetView(ViewSpan.Week);
                    break;
            }
        }
    }

    #endregion
}
