using CalendarCommon;

namespace CalendarClient
{
    /// <summary>
    /// Interface for calendar commands, defining common methods for execution, argument validation, copying, and help.
    /// </summary>
    public interface ICalendarCommand
    {
        public string CommandString { get; set; }

        // Method to execute the command.
        void Execute();

        // Method to check validity of command arguments.
        bool CheckArguments();

        // Method to create a copy of the command instance.
        ICalendarCommand Copy();

        // Method to retrieve help text for the command.
        string Help();
    }

    /// <summary>
    /// Command to create new user in the calendar system.
    /// </summary>
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
        public string Help() => "createUser: Creates a new user in the calendar system.";
    }

    /// <summary>
    /// Command to change name of the current user.
    /// </summary>
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

        public string Help() => "changeName: Changes the name of the current user.";

        public void Execute()
        {
            User user = ui.ChangeUserName();
            user.password = client.user.password;
            if (connection.ChangeUser(user))
            {
                client.user = user;
                ui.UserName = user.name!;
            }
            else
            {
                ui.ShowMessage("Username is already taken. Choose different.");
            }
        }
    }

    /// <summary>
    /// Command to change password of the current user.
    /// </summary>
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

        public string Help() => "changePassword: Changes the password of the current user.";

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

    /// <summary>
    /// Command to log the user into the calendar system.
    /// </summary>
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

        public string Help() => "login: Logs the user into the calendar system.";

        public void Execute()
        {
            User user = ui.LoginUser();
            if (connection.SetClientUser(user))
            {
                client.user = user;
                ui.UserName = user.name!;
            }
            else
            {
                connection.SetClientDefaultUser();
                ui.ShowMessage("Invalid username or password.");
            }
        }
    }

    /// <summary>
    /// Command to log the user out of the calendar system.
    /// </summary>
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

        public string Help() => "logout: Logs the user out of the calendar system.";

        public void Execute()
        {
            ui.LogoutUser();
            connection.SetClientDefaultUser();
        }
    }

    /// <summary>
    /// Command to list events with their basic information.
    /// </summary>
    public class ListEventsCommand : ICalendarCommand
    {
        protected IUserInterface ui;
        protected IConnection connection;
        protected Client client;
        public string CommandString { get; set; }
        private List<CalendarEventBasic> events;
        public ListEventsCommand(IUserInterface ui, IConnection connection, Client client)
        {
            this.ui = ui;
            this.connection = connection;
            this.client = client;
            CommandString = "";
            events = new List<CalendarEventBasic>();
        }
        public virtual bool CheckArguments()
        {
            string[] args = CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (args.Length != 1 && args.Length != 2)
            {
                return false;
            }
            DateTime date;
            if (args.Length == 2 && !DateTime.TryParse(args[1], out date))
            {
                return false;
            }
            return true;
        }

        public virtual ICalendarCommand Copy() => new ListEventsCommand(ui, connection, client);

        public virtual string Help() => "list: Lists the events in the user's calendar.";

        public virtual void Execute()
        {
            string[] args = CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (args.Length == 2)
            {
                DateTime shownDate = DateTime.Parse(args[1]);
                client.shownDate = shownDate;
            }
            ListEvents();
        }
        /// <summary>
        /// Listing events in separate method to reusablility in derived classes.
        /// </summary>
        protected void ListEvents()
        {
            IEventList<CalendarEventBasic> events = connection.GetEvents();
            
            DateTime beginning = client.BeginningDate();
            int numberOfDays = client.NumberOfDays();

            if (numberOfDays > 0)
            {
                DateTime end = beginning.AddDays(numberOfDays);
                var filteredEvents = from calendarEvent in events where calendarEvent.beginning >= beginning where calendarEvent.beginning < end select calendarEvent;

                foreach(var calEvent in filteredEvents)
                {
                    this.events.Add(calEvent);
                }
            }
            else
            {
                var filteredEvents = from calendarEvent in events where calendarEvent.beginning >= beginning select calendarEvent;
                var iterator = filteredEvents.GetEnumerator();
                for(int i = 0; i < 10 && iterator.MoveNext();i++)
                {
                    this.events.Add(iterator.Current);
                }
            }
            ui.ListEvents(this.events);
        }
        public List<CalendarEventBasic> GetEvents() => events;
    }

    /// <summary>
    /// Command to add new event to the calendar.
    /// After saving new event, command lists events to show updated calendar.
    /// </summary>
    public class AddEventCommand : ListEventsCommand, ICalendarCommand
    {
        public AddEventCommand(IUserInterface ui, IConnection connection, Client client)
            : base(ui, connection, client) {}
        public override bool CheckArguments() => CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Length == 1;

        public override ICalendarCommand Copy() => new AddEventCommand(ui, connection, client);

        public override string Help() => "add: Adds a new event to the calendar.";

        public override void Execute()
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
            ListEvents();
        }
    }

    /// <summary>
    /// Command to delete event from the calendar
    /// After deleting event, command lists events to show updated calendar.
    /// </summary>
    public class DeleteEventCommand : ListEventsCommand, ICalendarCommand
    {
        public DeleteEventCommand(IUserInterface ui, IConnection connection, Client client)
            : base(ui, connection, client) {}
        public override bool CheckArguments()
        {
            var args = CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (args.Length != 2)
            {
                return false;
            }
            int number;
            if (!int.TryParse(args[1], out number))
            {
                return false;
            }
            return number >= 0 && number < client.ListLength();
        }

        public override ICalendarCommand Copy() => new DeleteEventCommand(ui, connection, client);

        public override string Help() => "delete [id]: Deletes the event with the specified ID.";

        public override void Execute()
        {
            int index = int.Parse(CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[1]);
            CalendarEventBasic calendarEvent = client.GetListedEvent(index);
            if (!calendarEvent.IsValid())
            {
                ui.ShowMessage("Unable to delete event.");
                return;
            }
            if (!ui.DeleteEvent(calendarEvent))
            {
                return;
            }
            connection.DeleteEvent((DateTime)calendarEvent.beginning!, (int)calendarEvent.id!);
            ListEvents();
        }
    }

    /// <summary>
    /// Command to edit existing event.
    /// After saving edited event, command lists events to show updated calendar.
    /// </summary>
    public class EditEventCommand : ListEventsCommand, ICalendarCommand
    {
        public EditEventCommand(IUserInterface ui, IConnection connection, Client client)
            : base(ui, connection, client) { }
        public override bool CheckArguments()
        {
            var args = CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (args.Length != 2)
            {
                return false;
            }
            int number;
            if (!int.TryParse(args[1], out number))
            {
                return false;
            }
            return number >= 0 && number < client.ListLength();
        }

        public override ICalendarCommand Copy() => new EditEventCommand(ui, connection, client);

        public override string Help() => "edit [id]: Edits the event with the specified ID.";

        public override void Execute()
        {
            int index = int.Parse(CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[1]);
            CalendarEventBasic preview = client.GetListedEvent(index);
            CalendarEvent oldCalendarEvent;
            if (!preview.IsValid() || !connection.GetEvent((DateTime)preview.beginning!, (int)preview.id!, out oldCalendarEvent))
            {
                ui.ShowMessage("Unable to get full event info. Try it again.");
                return;
            }
            CalendarEvent newCalendarEvent = ui.EditEvent(oldCalendarEvent);

            // If the beginning date has changed, deletes origninal event
            if(newCalendarEvent.beginning?.Date != oldCalendarEvent.beginning?.Date)
            {
                if(!connection.DeleteEvent((DateTime)oldCalendarEvent.beginning?.Date!, (int)oldCalendarEvent.id!))
                {
                    ui.ShowMessage("Saving event was not successfull. Try it again.");
                    return;
                }
            }
            if (!connection.SaveEvent(newCalendarEvent))
            {
                ui.ShowMessage("Saving event was not successfull. Try it again.");
                return;
            }
            ListEvents();
        }
    }

    /// <summary>
    /// Command to show detail information about specific event.
    /// </summary>
    public class ShowEventCommand : ICalendarCommand
    {
        private IUserInterface ui;
        private IConnection connection;
        private Client client;
        public string CommandString { get; set; }
        public ShowEventCommand(IUserInterface ui, IConnection connection, Client client)
        {
            this.ui = ui;
            this.connection = connection;
            this.client = client;
            CommandString = "";
        }
        public bool CheckArguments()
        {
            var args = CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (args.Length != 2)
            {
                return false;
            }
            int number;
            if (!int.TryParse(args[1], out number))
            {
                return false;
            }
            return number >= 0 && number < client.ListLength();
        }

        public ICalendarCommand Copy() => new ShowEventCommand(ui, connection, client);

        public string Help() => "show [id]: Displays the details of the event with the specified ID.";

        public void Execute()
        {
            int index = int.Parse(CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[1]);
            CalendarEventBasic preview = client.GetListedEvent(index);
            CalendarEvent calendarEvent;
            
            if (!preview.IsValid() || !connection.GetEvent((DateTime)preview.beginning!, (int)preview.id!, out calendarEvent))
            {
                ui.ShowMessage("Unable to show full event info.");
                return;
            }
            ui.ShowEvent(calendarEvent);
        }
    }

    /// <summary>
    /// Command to duplicate specific event.
    /// After saving duplicated event, command lists events to show updated calendar.
    /// </summary>
    public class DuplicateEventCommand : ListEventsCommand, ICalendarCommand
    {
        public DuplicateEventCommand(IUserInterface ui, IConnection connection, Client client)
            : base(ui, connection, client) { }

        public override bool CheckArguments()
        {
            var args = CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (args.Length != 2)
            {
                return false;
            }
            int number;
            if (!int.TryParse(args[1], out number))
            {
                return false;
            }
            return number >= 0 && number < client.ListLength();
        }

        public override ICalendarCommand Copy() => new DuplicateEventCommand(ui, connection, client);

        public override string Help() => "duplicate [id]: Duplicates the event with the specified ID.";

        public override void Execute()
        {
            int index = int.Parse(CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[1]);
            CalendarEventBasic preview = client.GetListedEvent(index);
            CalendarEvent calendarEvent;
            if (!preview.IsValid() || !connection.GetEvent((DateTime)preview.beginning!, (int)preview.id!, out calendarEvent))
            {
                ui.ShowMessage("Unable to get full event info. Try it again.");
                return;
            }
            calendarEvent = ui.EditEvent(calendarEvent);
            calendarEvent.id = null;
            if (!connection.SaveEvent(calendarEvent))
            {
                ui.ShowMessage("Saving event was not successfull. Try it again.");
            }
            ListEvents();
        }
    }

    /// <summary>
    /// Command to list events for the next week or month, based on chosen time span.
    /// </summary>
    public class NextCommand : ListEventsCommand, ICalendarCommand
    {
        public NextCommand(IUserInterface ui, IConnection connection, Client client) : base(ui, connection, client) { }
        public override bool CheckArguments() => CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Length == 1;

        public override ICalendarCommand Copy() => new NextCommand(ui, connection, client);

        public override string Help() => "next: Shows the events for the next week/month.";

        public override void Execute()
        {
            client.shownDate = client.shownDate.AddDays(7);
            ListEvents();
        }
    }

    /// <summary>
    /// Command to list events for the previous week or month, based on chosen time span.
    /// </summary>
    public class PreviousCommand : ListEventsCommand, ICalendarCommand
    {
        public PreviousCommand(IUserInterface ui, IConnection connection, Client client) : base(ui, connection, client) { }
        public override bool CheckArguments() => CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Length == 1;

        public override ICalendarCommand Copy() => new PreviousCommand(ui, connection, client);

        public override string Help() => "previous: Shows the events for the previous week/month.";

        public override void Execute()
        {
            client.shownDate = client.shownDate.AddDays(-7);
            ListEvents();
        }
    }

    /// <summary>
    /// Command to list events for the current week or month, based on chosen time span.
    /// </summary>
    public class CurrentCommand : ListEventsCommand, ICalendarCommand
    {
        public CurrentCommand(IUserInterface ui, IConnection connection, Client client)
            : base(ui, connection, client) { }
        public override bool CheckArguments() => CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Length == 1;

        public override ICalendarCommand Copy() => new CurrentCommand(ui, connection, client);

        public override string Help() => "current: Shows the events for the current week/month.";

        public override void Execute()
        {
            client.shownDate = DateTime.Now;
            ListEvents();
        }
    }

    /// <summary>
    /// Command to change time span (week, month or upcomming) for which events are listed.
    /// </summary>
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
            if (args.Length != 2)
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

        public string Help() => "view [time span]: Views the events for a specified time span (week, month, or upcoming).";

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

    /// <summary>
    /// Command to exit the aplication.
    /// </summary>
    public class ExitCommand : ICalendarCommand
    {
        public string CommandString { get; set; }

        public ExitCommand()
        {
            CommandString = "exit";
        }

        public bool CheckArguments() => true;

        public ICalendarCommand Copy() => new ExitCommand();

        public string Help() => "exit: Exits the application.";

        public void Execute() { }
    }

    /// <summary>
    /// Command to show help information for all available commands.
    /// </summary>
    public class HelpCommand : ICalendarCommand
    {
        private IUserInterface ui;
        public string CommandString { get; set; }

        public HelpCommand(IUserInterface ui)
        {
            this.ui = ui;
            CommandString = "";
        }

        public bool CheckArguments() => CommandString.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Length == 1;

        public ICalendarCommand Copy() => new HelpCommand(ui);

        public string Help() => "help: Shows the help information for available commands.";

        public void Execute()
        {
            ui.Help();
        }
    }
}
