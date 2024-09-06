using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarClient
{
    public class Client
    {
        public Client()
        {
            Dictionary<string, ICalendarCommand> commands = new Dictionary<string, ICalendarCommand>();
            commands.Add("add", new AddEventCommand());
            ui = new CommandLineInterface(commands);
        }
        private IUserInterface ui;
        private IConnection connection;
        int viewSpan;
        ICalendarCommand lastCommand;
    }
}
