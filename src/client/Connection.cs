using CalendarCommon;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CalendarClient
{
    /// <summary>
    /// Interface that defines methods for managing user and calendar event data.
    /// This interface handles operations such as user management and event handling in the calendar system.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Sets the user for the client session.
        /// </summary>
        /// <param name="user">The user to be set for the session.</param>
        /// <returns>Returns true if the user is successfully set, false otherwise.</returns>
        public bool SetClientUser(User user);

        /// <summary>
        /// Resets the client session to the default user.
        /// </summary>
        public void SetClientDefaultUser();

        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="user">The user to be created.</param>
        /// <returns>Returns true if the user is successfully created, false otherwise.</returns>
        public bool CreateUser(User user);

        /// <summary>
        /// Changes the details of an existing user.
        /// </summary>
        /// <param name="newUser">The updated user with new details.</param>
        /// <returns>Returns true if the user details are successfully updated, false otherwise.</returns>
        public bool ChangeUser(User newUser);

        /// <summary>
        /// Saves a calendar event in the system.
        /// </summary>
        /// <param name="calendarEvent">The calendar event to be saved.</param>
        /// <returns>Returns true if the event is successfully saved, false otherwise.</returns>
        public bool SaveEvent(CalendarEvent calendarEvent);

        /// <summary>
        /// Retrieves a specific calendar event based on date and ID.
        /// </summary>
        /// <param name="date">The date of the event.</param>
        /// <param name="ID">The ID of the event.</param>
        /// <param name="calendarEvent">The output parameter that will contain the retrieved event if found.</param>
        /// <returns>Returns true if the event is found, false otherwise.</returns>
        public bool GetEvent(DateTime date, int ID, out CalendarEvent calendarEvent);

        /// <summary>
        /// Retrieves all calendar events for the current user.
        /// </summary>
        /// <returns>Returns an IEvent which can retrieve the basic details of calendar events.</returns>
        public IEvents<CalendarEventBasic> GetEvents();

        /// <summary>
        /// Deletes a specific calendar event based on date and ID.
        /// </summary>
        /// <param name="date">The date of the event.</param>
        /// <param name="ID">The ID of the event to be deleted.</param>
        /// <returns>Returns true if the event is successfully deleted, false otherwise.</returns>
        public bool DeleteEvent(DateTime date, int ID);
    }

    /// <summary>
    /// Interface for managing calendar events.
    /// Extends IEnumerable to allow iteration over the event list and provides filtering functionality with LINQ syntax.
    /// </summary>
    /// <typeparam name="T">The type of events in the list, which must be derived from CalendarEventBasic.</typeparam>

    public interface IEvents<T> : IEnumerable<T> where T : CalendarEventBasic
    {

        /// <summary>
        /// Filters the event list based on a specified predicate.
        /// Supports only comparing date of the event beginning.
        /// </summary>
        /// <param name="expression">The predicate expression used to filter the events.</param>
        /// <returns>Returns a filtered IEvents containing events that match the predicate.</returns>

        public IEvents<T> Where(Expression<Predicate<T>> expression);
    }
}
