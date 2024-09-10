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
            commandHistory = new List<ICalendarCommand>();
            commandHistoryIndexer = 0;
        }

        public ICalendarCommand GetInput()
        {
                StringBuilder sb = new StringBuilder();
                ConsoleKeyInfo key;

            while (true)
            {
                key = Console.ReadKey(true);
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
                            commandHistory.Add(command);
                            sb.Clear();
                            ClearLine();
                            commandHistoryIndexer = 0;
                            return command;
                        }
                        else
                        {
                            ShowMessage("Invalid arguments");
                            Console.Write(commandWhole);
                        }
                    }
                    else
                    {
                        ShowMessage("Invalid command");
                        Console.Write(commandWhole);
                    }
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        sb.Remove(sb.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    if (commandHistoryIndexer > 1)
                    {
                        commandHistoryIndexer--;
                        ICalendarCommand command = commandHistory[commandHistory.Count - commandHistoryIndexer];
                        sb = new StringBuilder(command.CommandString);
                        ClearLine();
                        Console.Write(sb.ToString());
                    }
                    else
                    {
                        commandHistoryIndexer = 0;
                        sb = new StringBuilder();
                        ClearLine();
                    }
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    if (commandHistoryIndexer < commandHistory.Count)
                    {
                        commandHistoryIndexer++;
                        ICalendarCommand command = commandHistory[commandHistory.Count - commandHistoryIndexer];
                        sb = new StringBuilder(command.CommandString);
                        ClearLine();
                        Console.Write(sb.ToString());
                    }
                }
                else if(Char.IsLetterOrDigit(key.KeyChar) || char.IsWhiteSpace(key.KeyChar))
                {
                    sb.Append(key.KeyChar);
                    Console.Write(key.KeyChar);
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
            Console.WriteLine(message);
        }

        private void ClearLine()
        {
            var a = Console.GetCursorPosition();
            Console.SetCursorPosition(0, a.Top);
            for (int i = 0; i < Console.BufferWidth; i++)
            {
                Console.Write(' ');
            }
            Console.SetCursorPosition(0, a.Top);
        }
    }
}
