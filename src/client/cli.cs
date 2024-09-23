using System.Text;
using CalendarCommon;

namespace CalendarClient
{
    /// <summary>
    /// Class that implements the IUserInterface interface to interact with the user via the command line.
    /// This class handles user input, command execution, event management, and provides a text-based user interface.
    /// </summary>
    public class CommandLineInterface : IUserInterface
    {
        private Dictionary<string, ICalendarCommand> commands;
        private List<ICalendarCommand> commandHistory;
        private int commandHistoryIndexer;
        public string UserName { get; set; }
        public CommandLineInterface()
        {
            commands = new();
            commandHistory = new List<ICalendarCommand>();
            commandHistoryIndexer = 0;
            UserName = string.Empty;
        }

        /// <summary>
        /// Waits for user input from the command line, processes the input, and returns the corresponding command.
        /// Provides tab completion, history navigation, and error handling for invalid commands.
        /// </summary>
        /// <returns>Returns an ICalendarCommand object representing the user's command input.</returns>
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

        /// <summary>
        /// Prompts the user to create a new user by entering their name and password.
        /// </summary>
        /// <returns>Returns a User object with the user's details.</returns>
        public User CreateUser()
        {
            string name = GetValidStringProperty("Name");
            User user = GetValidPassword();
            user.name = name;
            return user;
        }

        /// <summary>
        /// Prompts the user to change their username.
        /// </summary>
        /// <returns>Returns a User object with the new username.</returns>
        public User ChangeUserName()
        { 
            string name = GetValidStringProperty("New name:");
            return new User() { name = name };
        }

        /// <summary>
        /// Prompts the user to change their password.
        /// </summary>
        /// <returns>Returns a User object with the new password.</returns>
        public User ChangeUserPassword()
        {
            return GetValidPassword();
        }

        /// <summary>
        /// Prompts the user to log in by entering their username and password.
        /// </summary>
        /// <returns>Returns a User object with the entered credentials.</returns>
        public User LoginUser()
        {
            Console.WriteLine("Name:");
            string? name = Console.ReadLine();
            Console.WriteLine("Password:");
            string password;
            ReadPassword(out password);
            Console.WriteLine();
            return new User() { name = name, password = password };
        }

        /// <summary>
        /// Logs the user out, clears the username, and clears the command history.
        /// </summary>
        public void LogoutUser()
        {
            UserName = string.Empty;
            commandHistory.Clear();
        }

        /// <summary>
        /// Prompts the user to create a new calendar event by entering the necessary details.
        /// </summary>
        /// <returns>Returns a CalendarEvent object with the entered event details.</returns>
        public CalendarEvent AddEvent()
        {
            CalendarEvent calendarEvent = new CalendarEvent();
            calendarEvent.name = GetValidStringProperty("Name");

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

        /// <summary>
        /// Prompts the user to confirm deletion of a calendar event.
        /// </summary>
        /// <param name="calendarEvent">The event to be deleted.</param>
        /// <returns>Returns true if the user confirms deletion; otherwise, false.</returns>
        public bool DeleteEvent(CalendarEventBasic calendarEvent)
        {
            Console.WriteLine($"{calendarEvent.beginning?.ToShortDateString()}, {calendarEvent.beginning?.ToShortTimeString()}, {calendarEvent.name}");
            Console.Write("Do you want to delete this event?[N/y]:  ");
            if(Console.ReadLine() == "y")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Prompts the user to edit an existing calendar event by entering new or updated details.
        /// </summary>
        /// <param name="oldCalendarEvent">The existing calendar event to be edited.</param>
        /// <returns>Returns the updated CalendarEvent object.</returns>
        public CalendarEvent EditEvent(CalendarEvent oldCalendarEvent)
        {
            CalendarEvent newCalendarEvent = new CalendarEvent();
            newCalendarEvent.id = oldCalendarEvent.id;

            Console.WriteLine("Name:");
            string name = ReadStringWithExample(oldCalendarEvent.name!);
            newCalendarEvent.name =  string.IsNullOrEmpty(name) ? oldCalendarEvent.name : name;

            Console.WriteLine("Beginning:");
            newCalendarEvent.beginning = ReadDateTime((DateTime)oldCalendarEvent.beginning!);

            newCalendarEvent.end = GetValidEndDate((DateTime)oldCalendarEvent.beginning!, (DateTime)oldCalendarEvent.end!);

            Console.WriteLine("Place:");
            string place = ReadStringWithExample(oldCalendarEvent.place ?? "");
            newCalendarEvent.place = string.IsNullOrEmpty(place) ? oldCalendarEvent.place : place;

            Console.WriteLine("Description:");
            string description = ReadStringWithExample(oldCalendarEvent.eventDescription ?? "");
            newCalendarEvent.eventDescription = string.IsNullOrEmpty(description) ? oldCalendarEvent.eventDescription : description;

            return newCalendarEvent;
        }

        /// <summary>
        /// Displays a list of calendar events to the user.
        /// </summary>
        /// <param name="events">The list of events to display.</param>
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
                Console.WriteLine($"{index++}     {calendarEvent.beginning?.ToShortDateString()}, {calendarEvent.beginning?.ToShortTimeString()} - {calendarEvent.end?.ToShortDateString()}, {calendarEvent.end?.ToShortTimeString()}, {calendarEvent.name}");
            }
        }

        /// <summary>
        /// Displays help information about available commands.
        /// </summary>
        public void Help()
        {
            foreach(var command in commands.Values)
            {
                Console.WriteLine(command.Help());
            }
        }

        /// <summary>
        /// Displays the details of a specific calendar event.
        /// </summary>
        /// <param name="calendarEvent">The event to display.</param>
        public void ShowEvent(CalendarEvent calendarEvent)
        {
            Console.WriteLine($"Beginning:       {calendarEvent.beginning?.ToShortDateString()}, {calendarEvent.beginning?.ToShortTimeString()}");
            Console.WriteLine($"End:             {calendarEvent.end?.ToShortDateString()}, {calendarEvent.end?.ToShortTimeString()}");
            Console.WriteLine($"Name:            {calendarEvent.name}");
            Console.WriteLine($"Place:           {calendarEvent.place}");
            Console.WriteLine($"Description:\n   {calendarEvent.eventDescription}");
        }

        /// <summary>
        /// Displays a message to the user.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Sets the available commands for the user interface.
        /// </summary>
        /// <param name="commands">A dictionary of command names and corresponding command objects.</param>
        public void SetCommands(Dictionary<string, ICalendarCommand> commands)
        {
            this.commands = commands;
        }

        /// <summary>
        /// Clears the current console line.
        /// </summary>
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

        /// <summary>
        /// Deletes the last character from the input.
        /// </summary>
        /// <param name="sb">The StringBuilder containing the current input.</param>
        private void DeleteLastChar(StringBuilder sb)
        {
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
                Console.Write("\b \b");
            }
        }

        /// <summary>
        /// Prompts the user to enter a valid string property (such as a name).
        /// </summary>
        /// <param name="propertyName">The name of the property being requested.</param>
        /// <returns>Returns the valid string entered by the user.</returns>
        private string GetValidStringProperty(string propertyName)
        {
            while (true)
            {
                Console.WriteLine($"{propertyName}:");
                string? value = Console.ReadLine();
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
                var cursorPos = Console.CursorTop;
                for (int i = 1; i <= 2; i++)
                {
                    Console.SetCursorPosition(0, cursorPos - i);
                    ClearLine();
                }
                Console.WriteLine("Name is not valid. Try again.");
            }
        }

        /// <summary>
        /// Prompts the user to enter and confirm a valid password.
        /// </summary>
        /// <returns>Returns a User object with the entered password.</returns>
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

        /// <summary>
        /// Reads the user's password input, hiding the actual characters.
        /// </summary>
        /// <param name="password">The output string for the password entered.</param>
        /// <returns>Returns true if the password is successfully entered; otherwise, false.</returns>
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

        /// <summary>
        /// Writes the command prompt with the current username.
        /// </summary>
        private void WritePrompt()
        {
            Console.Write(UserName is null ? "Calendar@ " : $"Calendar@{UserName} ");
        }

        /// <summary>
        /// Reads a string input from the user with an example value displayed.
        /// </summary>
        /// <param name="example">The example value to display to the user.</param>
        /// <returns>Returns the string entered by the user or the example if no input is provided.</returns>
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

        /// <summary>
        /// Prompts the user to enter a valid end date for a calendar event.
        /// Ensures the end date is later than the beginning date.
        /// </summary>
        /// <param name="beginning">The beginning date of the event.</param>
        /// <param name="example">An example end date to display.</param>
        /// <returns>Returns the valid end date entered by the user.</returns>
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

        /// <summary>
        /// Prompts the user to enter a valid date and time, using an example as a default value.
        /// </summary>
        /// <param name="example">The example date and time to display.</param>
        /// <returns>Returns the valid DateTime entered by the user.</returns>
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

        /// <summary>
        /// Tries to parse a date and time from a string input.
        /// </summary>
        /// <param name="input">The string input to parse.</param>
        /// <param name="example">An example DateTime value to use as a default.</param>
        /// <param name="result">The parsed DateTime output.</param>
        /// <returns>Returns true if the parsing is successful; otherwise, false.</returns>
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

        /// <summary>
        /// Moves down in the command history and retrieves the next command.
        /// </summary>
        /// <param name="sb">The StringBuilder to store the retrieved command.</param>
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

        /// <summary>
        /// Moves up in the command history and retrieves the previous command.
        /// </summary>
        /// <param name="sb">The StringBuilder to store the retrieved command.</param>
        private void MoveHistoryUp(StringBuilder sb)
        {
            if (commandHistoryIndexer < commandHistory.Count)
            {
                commandHistoryIndexer++;
                GetCommandFromHistory(sb);
            }
        }

        /// <summary>
        /// Retrieves a command from the command history and displays it in the input prompt.
        /// </summary>
        /// <param name="sb">The StringBuilder to store the retrieved command.</param>
        private void GetCommandFromHistory(StringBuilder sb)
        {
            ICalendarCommand command = commandHistory[commandHistory.Count - commandHistoryIndexer];
            sb.Clear();
            sb.Append(command.CommandString);
            ClearLine();
            WritePrompt();
            Console.Write(sb.ToString());
        }

        /// <summary>
        /// Provides tab completion for partially typed commands by finding matching commands.
        /// </summary>
        /// <param name="sb">The StringBuilder containing the user's current input.</param>
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

        /// <summary>
        /// Finds the longest common prefix of a list of strings.
        /// </summary>
        /// <param name="commands">The list of command strings to compare.</param>
        /// <returns>Returns the longest common prefix of the provided commands.</returns>
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
