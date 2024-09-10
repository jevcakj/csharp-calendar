using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalendarCommon;

namespace CalendarClient
{
    public class Client
    {
        private IUserInterface ui;
        private IConnection connection;
        int viewSpan;
        ICalendarCommand lastCommand;
        public Client()
        {
            Dictionary<string, ICalendarCommand> commands = new Dictionary<string, ICalendarCommand>();
            ui = new CommandLineInterface(commands);
            connection = new HttpConnection(new User() { name = "defaultUser", password = "" });

            commands.Add("createUser", new CreateUserCommand(ui, connection));
            commands.Add("login", new LoginUserCommand(ui, connection));
            commands.Add("logout", new LogoutUserCommand(ui, connection));
            commands.Add("changeName", new ChangeUserNameCommand(ui, connection));
            commands.Add("changePassword", new ChangeUserPasswordCommand(ui, connection));
            commands.Add("add", new AddEventCommand(ui, connection));
            commands.Add("delete", new DeleteEventCommand(ui, connection));
            commands.Add("edit", new EditEventCommand(ui, connection));
            commands.Add("list", new ListEventsCommand(ui, connection));
            commands.Add("show", new ShowEventCommand(ui,connection));
            commands.Add("next", new NextCommand(ui, connection));
            commands.Add("previous", new PreviousCommand(ui, connection));
            commands.Add("current", new CurrentCommand(ui, connection));
            commands.Add("view", new ViewCommand(ui, connection));
        }

    }
}
