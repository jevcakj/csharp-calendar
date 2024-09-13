using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CalendarCommon;
using Microsoft.Win32.SafeHandles;

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
        public User user { get;  set; }
        public DateTime shownDate { get; set; }
        ViewSpan viewSpan;
        List<CalendarEvent>? eventsListed;
        public Client()
        {
            Dictionary<string, ICalendarCommand> commands = new Dictionary<string, ICalendarCommand>();
            ui = new CommandLineInterface(commands);
            connection = new HttpConnection(new User() { name = "defaultUser", password = "" });
            shownDate = DateTime.Now;
            viewSpan = ViewSpan.Week;

            commands.Add("createUser", new CreateUserCommand(ui, connection));
            commands.Add("login", new LoginUserCommand(ui, connection, this));
            commands.Add("logout", new LogoutUserCommand(ui, connection));
            commands.Add("changeName", new ChangeUserNameCommand(ui, connection, this));
            commands.Add("changePassword", new ChangeUserPasswordCommand(ui, connection, this));
            commands.Add("add", new AddEventCommand(ui, connection));
            commands.Add("delete", new DeleteEventCommand(ui, connection));
            commands.Add("edit", new EditEventCommand(ui, connection));
            commands.Add("list", new ListEventsCommand(ui, connection, this));
            commands.Add("show", new ShowEventCommand(ui,connection));
            commands.Add("next", new NextCommand(ui, connection));
            commands.Add("previous", new PreviousCommand(ui, connection));
            commands.Add("current", new CurrentCommand(ui, connection));
            commands.Add("view", new ViewCommand(ui, connection, this));
            commands.Add("exit", new ExitCommand());
        }

        public void Start()
        {
            while (true)
            {
                ICalendarCommand command = ui.GetInput();
                if(command is ExitCommand)
                {
                    return;
                }
                if(command is ListEventsCommand list)
                {
                    eventsListed = list.GetEvents();
                }
                command.Execute();
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
    }
}
