namespace CalendarCommon
{
    /// <summary>
    /// Class representing a user in the calendar system.
    /// This class stores user-related information such as ID, name, and password.
    /// </summary>
    public class User
    {
        public int? id {  get; set; }
        public string? name { get; set; }
        public string? password { get; set; }

        public User() { }
    }
}
