using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            commands.Add("add", new AddEventCommand());
            ui = new CommandLineInterface(commands);
        }

    }
}
