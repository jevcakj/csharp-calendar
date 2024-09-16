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
        User defaultUser;
        public User user { get;  set; }
        public DateTime shownDate { get; set; }
        ViewSpan viewSpan;
        List<CalendarEventBasic> eventsListed;
        public Client()
        {
            Dictionary<string, ICalendarCommand> commands = new Dictionary<string, ICalendarCommand>();
            ui = new CommandLineInterface(commands);
            defaultUser = new User() { name = "defaultUser", password = "" };
            user = defaultUser;
            connection = new HttpConnection(defaultUser);
            shownDate = DateTime.Now;
            viewSpan = ViewSpan.Week;
            eventsListed = new List<CalendarEventBasic>();

            commands.Add("createUser", new CreateUserCommand(ui, connection));
            commands.Add("login", new LoginUserCommand(ui, connection, this));
            commands.Add("logout", new LogoutUserCommand(ui, connection));
            commands.Add("changeName", new ChangeUserNameCommand(ui, connection, this));
            commands.Add("changePassword", new ChangeUserPasswordCommand(ui, connection, this));
            commands.Add("add", new AddEventCommand(ui, connection));
            commands.Add("delete", new DeleteEventCommand(ui, connection, this));
            commands.Add("edit", new EditEventCommand(ui, connection, this));
            commands.Add("list", new ListEventsCommand(ui, connection, this));
            commands.Add("show", new ShowEventCommand(ui,connection, this));
            commands.Add("next", new NextCommand(ui, connection, this));
            commands.Add("previous", new PreviousCommand(ui, connection, this));
            commands.Add("current", new CurrentCommand(ui, connection, this));
            commands.Add("view", new ViewCommand(ui, connection, this));
            commands.Add("exit", new ExitCommand());
        }

        public void Start()
        {

            //TODO napsat funkci userloggedid obalenou 
            while (true)
            {
                ICalendarCommand command = ui.GetInput();
                if(command is ExitCommand)
                {
                    return;
                }
                if(command is LogoutUserCommand)
                {
                    user = defaultUser;
                }
                if(command is ListEventsCommand list)
                {
                    eventsListed = list.GetEvents();
                }
                command.Execute();

                if (command is LoginUserCommand)
                {
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
