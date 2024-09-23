using CalendarCommon;

namespace CalendarServer
{
    /// <summary>
    /// Interface that defines the data access operations for managing users and calendar events.
    /// This interface handles creating, updating, authenticating users, and managing events in the system.
    /// </summary>
    public interface IData
    {
        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="user">The user object containing the user's details to be created.</param>
        /// <returns>Returns true if the user is successfully created, otherwise false.</returns>
        public bool CreateUser(User user);

        /// <summary>
        /// Updates the username of an existing user.
        /// </summary>
        /// <param name="oldUser">The existing user whose name is to be updated.</param>
        /// <param name="newUser">The user object containing the new username.</param>
        /// <returns>Returns true if the username is successfully updated, otherwise false.</returns>
        public bool UpdateUserName(User oldUser, User newUser);

        /// <summary>
        /// Updates the password of an existing user.
        /// </summary>
        /// <param name="oldUser">The existing user whose password is to be updated.</param>
        /// <param name="newUser">The user object containing the new password.</param>
        /// <returns>Returns true if the password is successfully updated, otherwise false.</returns>
        public bool UpdateUserPassword(User oldUser, User newUser);

        /// <summary>
        /// Authenticates a user by verifying their credentials.
        /// </summary>
        /// <param name="user">The user object containing the credentials for authentication.</param>
        /// <returns>Returns true if the user is authenticated, otherwise false.</returns>
        public bool AuthenticateUser(User user);

        /// <summary>
        /// Saves a calendar event for a specific user.
        /// </summary>
        /// <param name="e">The calendar event to be saved.</param>
        /// <param name="user">The user who owns the event.</param>
        /// <returns>Returns the event's ID after it is saved.</returns>
        public int SaveEvent(CalendarEvent e, User user);

        /// <summary>
        /// Deletes a specific calendar event for a user based on its date and ID.
        /// </summary>
        /// <param name="dateTime">The date of the event to be deleted.</param>
        /// <param name="id">The ID of the event to be deleted.</param>
        /// <param name="user">The user who owns the event.</param>
        public void DeleteEvent(DateTime dateTime, int id, User user);

        /// <summary>
        /// Retrieves all calendar events for a user on a specific date.
        /// </summary>
        /// <param name="dateTime">The date for which events are to be retrieved.</param>
        /// <param name="user">The user whose events are being retrieved.</param>
        /// <param name="calendarEvents">The list of calendar events (output parameter).</param>
        /// <returns>Returns true if events are successfully retrieved, otherwise false.</returns>
        public bool GetEvents(DateTime dateTime, User user, out List<CalendarEventBasic> calendarEvents);

        /// <summary>
        /// Retrieves a specific calendar event for a user based on its date and ID.
        /// </summary>
        /// <param name="dateTime">The date of the event.</param>
        /// <param name="id">The ID of the event.</param>
        /// <param name="user">The user who owns the event.</param>
        /// <param name="calendarEvent">The calendar event object (output parameter).</param>
        /// <returns>Returns true if the event is successfully retrieved, otherwise false.</returns>
        public bool GetEvent(DateTime dateTime, int id, User user, out CalendarEvent calendarEvent);
    }
}
