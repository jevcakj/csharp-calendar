using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalendarCommon;

namespace CalendarClient
{
    public class CommandLineInterface : IUserInterface
    {
        private Dictionary<string, ICalendarCommand> commands;
        private List<ICalendarCommand> commandHistory;
        private int commandHistoryIndexer;
        public CommandLineInterface(Dictionary<string, ICalendarCommand> commands)
        {
            this.commands = commands;
        }

        public ICalendarCommand GetInput()
        {
                StringBuilder sb = new StringBuilder();
                ConsoleKeyInfo key;

            while (true)
            {
                key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                {
                    string commandWhole = sb.ToString();
                    string commandName = commandWhole.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[0];
                    ICalendarCommand? commandPrototype;
                    commands.TryGetValue(commandName, out commandPrototype);
                    if (commandPrototype != null)
                    {
                        ICalendarCommand command = commandPrototype.Copy();
                        command.CommandString = commandWhole;
                        if (command.CheckArguments())
                        {
                            return command;
                        }
                        else
                        {
                            ShowMessage("Invalid arguments");
                        }
                    }
                    else
                    {
                        ShowMessage("Invalid command");
                    }
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    sb.Remove(sb.Length-1, 1);
                    Console.Write('\b');
                }
                else
                {
                    sb.Append(key.KeyChar);
                }
            }
        }

        public void AddEvent()
        {
            throw new NotImplementedException();
        }

        public User ChangeUserName()
        {
            throw new NotImplementedException();
        }

        public User ChangeUserPassword()
        {
            throw new NotImplementedException();
        }

        public User CreateUser()
        {
            throw new NotImplementedException();
        }

        public void DeleteEvent()
        {
            throw new NotImplementedException();
        }

        public void EditEvent()
        {
            throw new NotImplementedException();
        }

        public void ListEvents()
        {
            throw new NotImplementedException();
        }

        public User LoginUser()
        {
            throw new NotImplementedException();
        }

        public void LogoutUser()
        {
            throw new NotImplementedException();
        }

        public void ShowEvent()
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string message)
        {
            throw new NotImplementedException();
        }
    }
}
