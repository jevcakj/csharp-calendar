using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CalendarCommon;

namespace CalendarClient
{
    public class Client
    {
        private IUserInterface ui;
        private IConnection connection;
        public User user { get;  set; }
        int viewSpan;
        ICalendarCommand lastCommand;
        public Client()
        {
            Dictionary<string, ICalendarCommand> commands = new Dictionary<string, ICalendarCommand>();
            ui = new CommandLineInterface(commands);
            connection = new HttpConnection(new User() { name = "defaultUser", password = "" });

            commands.Add("createUser", new CreateUserCommand(ui, connection));
            commands.Add("login", new LoginUserCommand(ui, connection, this));
            commands.Add("logout", new LogoutUserCommand(ui, connection));
            commands.Add("changeName", new ChangeUserNameCommand(ui, connection, this));
            commands.Add("changePassword", new ChangeUserPasswordCommand(ui, connection, this));
            commands.Add("add", new AddEventCommand(ui, connection));
            commands.Add("delete", new DeleteEventCommand(ui, connection));
            commands.Add("edit", new EditEventCommand(ui, connection));
            commands.Add("list", new ListEventsCommand(ui, connection));
            commands.Add("show", new ShowEventCommand(ui,connection));
            commands.Add("next", new NextCommand(ui, connection));
            commands.Add("previous", new PreviousCommand(ui, connection));
            commands.Add("current", new CurrentCommand(ui, connection));
            commands.Add("view", new ViewCommand(ui, connection));
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
                command.Execute();
            }
        }
    }
}
