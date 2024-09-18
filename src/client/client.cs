using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CalendarCommon;
using Microsoft.Win32.SafeHandles;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CalendarClient
{
    public enum ViewSpan
    {
        Week,
        Month,
        Upcomming
    }
    public class Client
    {
        private IUserInterface ui;
        private IConnection connection;
        private User defaultUser;
        public User user { get;  set; }

        private Dictionary<string, ICalendarCommand> defaultCommands;
        private Dictionary<string, ICalendarCommand> userCommands;

        public DateTime shownDate { get; set; }
        ViewSpan viewSpan;
        List<CalendarEventBasic> eventsListed;

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
                { "add", new AddEventCommand(ui, connection) },
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
                if(command is ListEventsCommand list)
                {
                    eventsListed = list.GetEvents();
                }

                command.Execute();

                if (command is LoginUserCommand)
                {
                    ((CommandLineInterface)ui).SetCommands(userCommands);
                    ListEventsCommand listEvents = new ListEventsCommand(ui, connection, this);
                    listEvents.Execute();
                    eventsListed = listEvents.GetEvents();
                }
            }
        }

        public void SetView(ViewSpan viewSpan)
        {
            this.viewSpan = viewSpan;
        }

        public DateTime BeginningDate()
        {
            if(viewSpan == ViewSpan.Week)
            {
                var day = shownDate.DayOfWeek;
                return shownDate.AddDays(-(((int)day + 6) % 7));
            }
            else if(viewSpan == ViewSpan.Month)
            {
                return new DateTime(shownDate.Year, shownDate.Month, 1);
            }
            else
            {
                return shownDate;
            }
        }

        public int NumberOfDays()
        {
            switch(viewSpan)
            {
                case ViewSpan.Week:
                    return 7;
                case ViewSpan.Upcomming:
                    return -1;
                case ViewSpan.Month:
                    return DateTime.DaysInMonth(shownDate.Year, shownDate.Month);
                default:
                    return 7;
            }
        }
        public int ListLength() => eventsListed.Count;
        public CalendarEventBasic GetListedEvent(int index) => eventsListed[index];
    }
}
