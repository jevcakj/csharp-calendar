using CalendarCommon;

namespace CalendarClient
{
    /// <summary>
    /// Interface that defines the user interface operations for interacting with the calendar system.
    /// This interface handles user management, event management, and user interaction through input and messages.
    /// </summary>
    public interface IUserInterface
    {
        /// <summary>
        /// Gets or sets the username of the currently logged-in user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Retrieves the next command input from the user.
        /// </summary>
        /// <returns>Returns an ICalendarCommand object representing the user's input command.</returns>
        public ICalendarCommand GetInput();

        /// <summary>
        /// Prompts the user to log in by providing their credentials.
        /// </summary>
        /// <returns>Returns a User object with the credentials provided by the user.</returns>
        public User LoginUser();

        /// <summary>
        /// Logs the current user out of the system.
        /// </summary>
        public void LogoutUser();

        /// <summary>
        /// Prompts the user to create a new user by providing necessary details.
        /// </summary>
        /// <returns>Returns a User object with the details of the newly created user.</returns>
        public User CreateUser();

        /// <summary>
        /// Prompts the user to change their username.
        /// </summary>
        /// <returns>Returns a User object with the updated username.</returns>
        public User ChangeUserName();

        /// <summary>
        /// Prompts the user to change their password.
        /// </summary>
        /// <returns>Returns a User object with the updated password.</returns>
        public User ChangeUserPassword();

        /// <summary>
        /// Prompts the user to add a new event to the calendar.
        /// </summary>
        /// <returns>Returns a CalendarEvent object with the details of the new event.</returns>
        public CalendarEvent AddEvent();

        /// <summary>
        /// Prompts the user to confirm the deletion of a specific calendar event.
        /// </summary>
        /// <param name="calendarEvent">The calendar event to be deleted.</param>
        /// <returns>Returns true if the user confirms deletion, false otherwise.</returns>
        public bool DeleteEvent(CalendarEventBasic calendarEvent);

        /// <summary>
        /// Prompts the user to edit an existing calendar event.
        /// </summary>
        /// <param name="calendarEvent">The calendar event to be edited.</param>
        /// <returns>Returns the updated CalendarEvent object with the new details provided by the user.</returns>
        public CalendarEvent EditEvent(CalendarEvent calendarEvent);

        /// <summary>
        /// Displays the details of a specific calendar event to the user.
        /// </summary>
        /// <param name="calendarEvent">The calendar event whose details are to be displayed.</param>
        public void ShowEvent(CalendarEvent calendarEvent);

        /// <summary>
        /// Displays a list of calendar events to the user.
        /// </summary>
        /// <param name="events">A list of CalendarEventBasic objects to be displayed.</param>
        public void ListEvents( List<CalendarEventBasic> events);

        /// <summary>
        /// Displays help information or usage instructions to the user.
        /// </summary>
        public void Help();

        /// <summary>
        /// Displays a message to the user, typically used for errors or notifications.
        /// </summary>
        /// <param name="message">The message to be displayed to the user.</param>
        public void ShowMessage(string message);
    }
}
