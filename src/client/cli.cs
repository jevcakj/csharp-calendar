using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CalendarCommon;

namespace CalendarClient
{
    public class CommandLineInterface : IUserInterface
    {
        private Dictionary<string, ICalendarCommand> commands;
        private List<ICalendarCommand> commandHistory;
        private int commandHistoryIndexer;
        public string? UserName { get; set; }
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
            WritePrompt();

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
                            commandHistoryIndexer = 0;
                            Console.WriteLine();
                            return command;
                        }
                        else
                        {
                            Console.WriteLine();
                            ShowMessage("Invalid arguments");
                            WritePrompt();
                            Console.Write(commandWhole);
                        }
                    }
                    else
                    {
                        Console.WriteLine();
                        ShowMessage("Invalid command");
                        WritePrompt();
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
                        WritePrompt();
                        Console.Write(sb.ToString());
                    }
                    else
                    {
                        commandHistoryIndexer = 0;
                        sb = new StringBuilder();
                        ClearLine();
                        WritePrompt();
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
                        WritePrompt();
                        Console.Write(sb.ToString());
                    }
                }
                else if(!Char.IsControl(key.KeyChar))
                {
                    sb.Append(key.KeyChar);
                    Console.Write(key.KeyChar);
                }
            }
        }

        public User CreateUser()
        {
            Console.WriteLine("Name:");
            string name = Console.ReadLine();

            User user = GetValidPassword();
            user.name = name;
            return user;
        }

        public User ChangeUserName()
        {
            Console.WriteLine("Name:");
            string name = Console.ReadLine();
            return new User() { name = name };
        }

        public User ChangeUserPassword()
        {
            return GetValidPassword();
        }

        public User LoginUser()
        {
            Console.WriteLine("Name:");
            string name = Console.ReadLine();
            Console.WriteLine("Password:");
            string password;
            ReadPassword(out password);
            Console.WriteLine();
            return new User() { name = name, password = password };
        }

        public void LogoutUser()
        {
            UserName = null;
            commandHistory.Clear();
        }

        public CalendarEvent AddEvent()
        {
            CalendarEvent calendarEvent = new CalendarEvent();
            Console.WriteLine("Name:");
            calendarEvent.name = Console.ReadLine();

            StringBuilder stringBuilder = new StringBuilder();

            Console.WriteLine("Date:");
            var position = Console.GetCursorPosition();
            Console.WriteLine("dd/mm/yyyy");
            Console.SetCursorPosition(position.Left, position.Top);
            stringBuilder.Append(Console.ReadLine());
            stringBuilder.Append(' ');

            Console.WriteLine("Time: ");
            position = Console.GetCursorPosition();
            Console.WriteLine("hh:mm");
            Console.SetCursorPosition(position.Left, position.Top);
            stringBuilder.Append(Console.ReadLine());

            DateTime dateTime;
            if(!DateTime.TryParse(stringBuilder.ToString(), out dateTime))
            {
                ShowMessage("Invalid time or date format");
                return null;
            }
            calendarEvent.beginning = dateTime;
            calendarEvent.end = dateTime.AddHours(1);

            Console.WriteLine("Place:");
            string place = Console.ReadLine();
            calendarEvent.place = place;

            Console.WriteLine("Description:");
            string description = Console.ReadLine();
            calendarEvent.eventDescription = description;

            return calendarEvent;
        }

        public bool DeleteEvent(CalendarEventBasic calendarEvent)
        {
            Console.WriteLine($"{((DateTime)calendarEvent.beginning).ToShortDateString()}, {((DateTime)calendarEvent.beginning).ToShortTimeString()}, {calendarEvent.name}");
            Console.Write("Do you want to delete this event?[N/y]:  ");
            if(Console.ReadLine() == "y")
            {
                return true;
            }
            return false;
        }

        public void EditEvent()
        {
            throw new NotImplementedException();
        }

        public void ListEvents( List<CalendarEventBasic> events)
        {
            if(events is null ||  events.Count == 0)
            {
                ShowMessage("No events found.");
                return;
            }
            int index = 0;
            foreach( CalendarEventBasic calendarEvent in events)
            {
                Console.WriteLine($"{index++}     {((DateTime)calendarEvent.beginning).ToShortDateString()}, {((DateTime)calendarEvent.beginning).ToShortTimeString()} - {((DateTime)calendarEvent.end).ToShortDateString()}, {((DateTime)calendarEvent.end).ToShortTimeString()}, {calendarEvent.name}");
            }
        }

        public void ShowEvent(CalendarEvent calendarEvent)
        {
            Console.WriteLine($"Beginning:       {((DateTime)calendarEvent.beginning).ToShortDateString()}, {((DateTime)calendarEvent.beginning).ToShortTimeString()}");
            Console.WriteLine($"End:             {((DateTime)calendarEvent.end).ToShortDateString()}, {((DateTime)calendarEvent.end).ToShortTimeString()}");
            Console.WriteLine($"Name:            {calendarEvent.name}");
            Console.WriteLine($"Place:           {calendarEvent.place}");
            Console.WriteLine($"Description:\n   {calendarEvent.eventDescription}");
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

        private User GetValidPassword()
        {
            while (true)
            {
                Console.WriteLine("Password:");
                string password;
                ReadPassword(out password);
                Console.WriteLine();
                Console.WriteLine("Check password:");
                string checkPassword;
                ReadPassword(out checkPassword);
                Console.WriteLine();
                if (password == checkPassword)
                {
                    return new User() { password = password };
                }
                var curr = Console.CursorTop;
                for (int i = 1; i <= 4; i++)
                {
                    Console.SetCursorPosition(0, curr - i);
                    ClearLine();
                }
                Console.WriteLine("Passwords are different. Try again.");
            }
        }

        private bool ReadPassword(out string password)
        {
            StringBuilder sb = new StringBuilder();
            ConsoleKeyInfo key;

            while(true)
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    password = sb.ToString();
                    return true;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    password = "";
                    return false;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        sb.Remove(sb.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                }
                else if (Char.IsLetterOrDigit(key.KeyChar) || char.IsWhiteSpace(key.KeyChar))
                {
                    sb.Append(key.KeyChar);
                    Console.Write('.');
                }
            }
        }

        private void WritePrompt()
        {
            Console.Write(UserName is null ? "Calendar@ " : $"Calendar@{UserName} ");
        }
    }
}
