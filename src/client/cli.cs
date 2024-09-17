using System;
using System.Collections.Generic;
using System.Data;
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
        public CommandLineInterface()
        {
            commands = new();
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
                    string commandName = commandWhole.Length == 0 ? "" : commandWhole.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[0];
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
                    DeleteLastChar(sb);
                }
                else if (key.Key == ConsoleKey.Tab)
                {
                    TabCompletion(sb);
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    MoveHistoryDown(sb);
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    MoveHistoryUp(sb);
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

            Console.WriteLine("Beginning:");
            DateTime beginning = ReadDateTime(DateTime.Now);
            calendarEvent.beginning = beginning;

            calendarEvent.end = GetValidEndDate( beginning, beginning.AddHours(1));

            Console.WriteLine("Place:");
            calendarEvent.place = Console.ReadLine();
            
            Console.WriteLine("Description:");
            calendarEvent.place = Console.ReadLine();

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

        public CalendarEvent EditEvent(CalendarEvent calendarEvent)
        {
            Console.WriteLine("Name:");
            string name = ReadStringWithExample(calendarEvent.name);
            if (!string.IsNullOrEmpty(name))    { calendarEvent.name = name; }

            Console.WriteLine("Beginning:");
            calendarEvent.beginning = ReadDateTime((DateTime)calendarEvent.beginning);

            calendarEvent.end = GetValidEndDate((DateTime)calendarEvent.beginning, (DateTime)calendarEvent.end);

            Console.WriteLine("Place:");
            string place = ReadStringWithExample(calendarEvent.place);
            if (!string.IsNullOrEmpty(place))   { calendarEvent.place = place; }

            Console.WriteLine("Description:");
            string description = ReadStringWithExample(calendarEvent.eventDescription);
            if(!string.IsNullOrEmpty(description)) { calendarEvent.eventDescription = description; }

            return calendarEvent;
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

        public void SetCommands(Dictionary<string, ICalendarCommand> commands)
        {
            this.commands = commands;
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

        private void DeleteLastChar(StringBuilder sb)
        {
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
                Console.Write("\b \b");
            }
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
                    DeleteLastChar(sb);
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

        private string ReadStringWithExample(string example)
        {
            var cursorePos = Console.GetCursorPosition();
            StringBuilder sb = new StringBuilder();
            ConsoleKeyInfo key;

            Console.Write(example);
            Console.SetCursorPosition(cursorePos.Left, cursorePos.Top);

            while (true)
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return sb.ToString();
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        sb.Remove(sb.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                    if (sb.Length == 0)
                    {
                        Console.Write(example);
                        Console.SetCursorPosition(cursorePos.Left, cursorePos.Top);
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    if (sb.Length == 0)
                    {
                        ClearLine();
                    }
                    sb.Append(key.KeyChar);
                    Console.Write(key.KeyChar);
                }
            }
        }

        private DateTime GetValidEndDate(DateTime beginning, DateTime example)
        {
            while (true)
            {
                Console.WriteLine("End:");
                DateTime end = ReadDateTime(example);
                if(beginning < end)
                {
                    return end;
                }
                var curr = Console.CursorTop;
                for (int i = 1; i <= 2; i++)
                {
                    Console.SetCursorPosition(0, curr - i);
                    ClearLine();
                }
                Console.WriteLine("End time must be greater than beginning time.");
            }
        }

        private DateTime ReadDateTime(DateTime example)
        {
            DateTime date;
            string exampleString = $"{example.ToShortDateString()} {example.ToShortTimeString()}";
            while (true)
            {
                string dateString = ReadStringWithExample(exampleString);
                if(dateString == null || dateString == "")
                {
                    dateString = exampleString;
                }
                if(TryParseDateTime(dateString, example, out date))
                {
                    return date;
                }
                else
                {
                    ShowMessage("Invalid date or time.");
                }
            }
        }

        private bool TryParseDateTime(string input, DateTime example,  out DateTime result)
        {
            var splitted = input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if(splitted.Length == 1)
            {
                DateOnly date;
                TimeOnly time;
                if(DateOnly.TryParse(input, out date))
                {
                    result = new(date, new TimeOnly(example.Hour, example.Minute));
                    return true;
                }
                else if(TimeOnly.TryParse(input, out time))
                {
                    result = new(new DateOnly(example.Year, example.Month, example.Day), time);
                    return true;
                }
                else
                {
                    result = new();
                    return false;
                }

            }
            else if(splitted.Length == 2)
            {
                if(DateTime.TryParse(input, out result))
                {
                    return true;
                }
                else
                {
                    result = new();
                    return false;
                }
            }
            else
            {
                result = new();
                return false;
            }
        }

        private void MoveHistoryDown(StringBuilder sb)
        {
            if (commandHistoryIndexer > 1)
            {
                commandHistoryIndexer--;
                GetCommandFromHistory(sb);
            }
            else
            {
                commandHistoryIndexer = 0;
                sb.Clear();
                ClearLine();
                WritePrompt();
            }
        }

        private void MoveHistoryUp(StringBuilder sb)
        {
            if (commandHistoryIndexer < commandHistory.Count)
            {
                commandHistoryIndexer++;
                GetCommandFromHistory(sb);
            }
        }

        private void GetCommandFromHistory(StringBuilder sb)
        {
            ICalendarCommand command = commandHistory[commandHistory.Count - commandHistoryIndexer];
            sb.Clear();
            sb.Append(command.CommandString);
            ClearLine();
            WritePrompt();
            Console.Write(sb.ToString());
        }

        private void TabCompletion(StringBuilder sb)
        {
            string prefix = sb.ToString();
            List<string> results = new List<string>();
            foreach (var item in commands.Keys)
            {
                if (item.StartsWith(prefix))
                {
                    results.Add(item);
                }
            }

            if(results.Count == 0)
            {
                return;
            }
            if(results.Count == 1)
            {
                sb.Clear();
                sb.Append(results[0]);
                ClearLine();
                WritePrompt();
                Console.Write(results[0]);
                return;
            }
            else
            {
                Console.WriteLine();
                foreach (var command in results)
                {
                    Console.Write($"{command} ");
                }
                Console.WriteLine();
                sb.Clear();
                sb.Append(LongestCommonPrefix(results));
                WritePrompt();
                Console.Write(sb.ToString());
            }
        }
        private string LongestCommonPrefix(List<string> commands)
        {
            commands.Sort();
            var first = commands.First();
            var last = commands.Last();
            int boundary = first.Length < last.Length ? first.Length : last.Length;
            for(int i = 0; i < boundary; i++)
            {
                if(first[i] != last[i])
                {
                    return first.Substring(0, i);
                }
            }
            return first.Substring(0, boundary);
        }
    }
}
