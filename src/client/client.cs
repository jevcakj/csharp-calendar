using CalendarCommon;

namespace CalendarClient
{
    /// <summary>
    /// Enum representing different time spans for viewing calendar events.
    /// </summary>
    public enum ViewSpan
    {
        Week,
        Month,
        Upcomming
    }

    /// <summary>
    /// The main client class that handles the interaction between the user interface, connection, and calendar events.
    /// Manages user commands, session state, and event listings.
    /// </summary>
    public class Client
    {
        private IUserInterface ui;
        private IConnection connection;
        private User defaultUser;
        public User user { get;  set; }

        private Dictionary<string, ICalendarCommand> defaultCommands;   // Commands available for non-logged-in users
        private Dictionary<string, ICalendarCommand> userCommands;      // Commands available for logged-in users

        public DateTime shownDate { get; set; }
        ViewSpan viewSpan;
        List<CalendarEventBasic> eventsListed;

        /// <summary>
        /// Initializes the client with default settings, sets up commands for both logged-in and non-logged-in users,
        /// and initializes the default user and connection.
        /// </summary>
        public Client()
        {
            ui = new CommandLineInterface();
            defaultUser = new User() { name = "defaultUser", password = "" };
            user = defaultUser;
            connection = new HttpConnection(defaultUser);
            shownDate = DateTime.Now;
            viewSpan = ViewSpan.Week;
            eventsListed = new List<CalendarEventBasic>();

            defaultCommands = new Dictionary<string, ICalendarCommand>()
            {
                { "help", new HelpCommand(ui) },
                { "createUser", new CreateUserCommand(ui, connection) },
                { "login", new LoginUserCommand(ui, connection, this) },
                { "exit", new ExitCommand() }
            };

            userCommands = new Dictionary<string, ICalendarCommand>()
            {
                { "help", new HelpCommand(ui) },
                { "logout", new LogoutUserCommand(ui, connection) },
                { "changeName", new ChangeUserNameCommand(ui, connection, this) },
                { "changePassword", new ChangeUserPasswordCommand(ui, connection, this) },
                { "add", new AddEventCommand(ui, connection, this) },
                { "edit", new EditEventCommand(ui, connection, this) },
                { "duplicate", new DuplicateEventCommand(ui, connection, this) },
                { "delete", new DeleteEventCommand(ui, connection, this) },
                { "show", new ShowEventCommand(ui,connection, this) },
                { "list", new ListEventsCommand(ui, connection, this) },
                { "next", new NextCommand(ui, connection, this) },
                { "previous", new PreviousCommand(ui, connection, this) },
                { "current", new CurrentCommand(ui, connection, this) },
                { "view", new ViewCommand(ui, connection, this) },
                { "exit", new ExitCommand() }
            };
            ((CommandLineInterface)ui).SetCommands(defaultCommands);
        }

        /// <summary>
        /// Starts the client session, continuously processes user input and executes corresponding commands.
        /// Handles login, logout, and event management commands.
        /// </summary>
        public void Start()
        {
            new HelpCommand(ui).Execute();
            while (true)
            {
                ICalendarCommand command = ui.GetInput();

                if(command is ExitCommand)
                {
                    return;
                }
                if(command is LogoutUserCommand)
                {
                    ((CommandLineInterface)ui).SetCommands(defaultCommands);
                    user = defaultUser;
                    shownDate = DateTime.Now;
                    viewSpan = ViewSpan.Week;
                    eventsListed = new();
                }

                command.Execute();

                if (command is ListEventsCommand list)
                {
                    eventsListed = list.GetEvents();
                }
                if (command is LoginUserCommand)
                {
                    ((CommandLineInterface)ui).SetCommands(userCommands);
                    ListEventsCommand listEvents = new ListEventsCommand(ui, connection, this);
                    listEvents.Execute();
                    eventsListed = listEvents.GetEvents();
                }
            }
        }

        /// <summary>
        /// Sets the current view span (Week, Month, or Upcoming) for displaying events.
        /// </summary>
        /// <param name="viewSpan">The ViewSpan to set (Week, Month, or Upcoming).</param>
        public void SetView(ViewSpan viewSpan)
        {
            this.viewSpan = viewSpan;
        }

        /// <summary>
        /// Calculates and returns the beginning date of the current view span.
        /// 
        /// </summary>
        /// <returns>For a week view, this returns the start of the week. For a month view, it returns the first day of the month.
        /// For upcomming view, it returns the currently shown date.</returns>
        public DateTime BeginningDate()
        {
            if(viewSpan == ViewSpan.Week)
            {
                // Calculate the beginning of the week (Monday)
                var day = shownDate.DayOfWeek;
                return shownDate.AddDays(-(((int)day + 6) % 7));
            }
            else if(viewSpan == ViewSpan.Month)
            {
                // Return the first day of the current month
                return new DateTime(shownDate.Year, shownDate.Month, 1);
            }
            else
            {
                // For upcoming view, use the current date
                return shownDate;
            }
        }

        /// <summary>
        /// Returns the number of days in the current view span.
        /// </summary>
        /// <returns>For a week view, this returns 7. For a month view, this returns the number of days in the month.
        /// For the upcoming view, it returns -1 (unlimited days).</returns>
        public int NumberOfDays()
        {
            switch(viewSpan)
            {
                case ViewSpan.Week:
                    return 7;
                case ViewSpan.Upcomming:
                    return -1;  // Unlimited days for upcoming view
                case ViewSpan.Month:
                    return DateTime.DaysInMonth(shownDate.Year, shownDate.Month);   // Total days in the month
                default:
                    return 7;   // Default to 7 days for week view
            }
        }

        /// <summary>
        /// Returns the number of events currently listed in the events list.
        /// </summary>
        /// <returns>The number of events in the list.</returns>
        public int ListLength() => eventsListed.Count;

        /// <summary>
        /// Retrieves a specific event from the events list based on its index.
        /// </summary>
        /// <param name="index">The index of the event to retrieve.</param>
        /// <returns>The CalendarEventBasic object at the specified index.</returns>
        public CalendarEventBasic GetListedEvent(int index) => eventsListed[index];
    }
}
