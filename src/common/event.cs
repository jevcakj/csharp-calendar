using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarCommon
{
    /// <summary>
    /// Represents calendar event with its basic and madatory properties.
    /// </summary>
    public class CalendarEventBasic : IComparable<CalendarEventBasic>
    {
        public int? id {  get; set; }
        public string? name { get; set; }
        public DateTime? beginning { get; set; }
        public DateTime? end {  get; set; }

        public CalendarEventBasic() { }

        public bool IsValid() => id != null && name != null && beginning != null && end != null;

        /// <summary>
        /// Compares the current CalendarEventBasic instance with another instance 
        /// based on beginning time, end time, and name in that order.
        /// </summary>
        /// <param name="other">The other CalendarEventBasic object to compare to.</param>
        /// <returns>Returns -1 if the current instance is less than the other, 1 if greater, or 0 if equal.</returns>
        public int CompareTo(CalendarEventBasic? other)
        {
            // If the other is null, this object is considered greater.
            if (other is null) return 1;

            int result = beginning?.CompareTo(other.beginning) ?? -1;
            if(result < 0) return -1;
            if(result > 0) return 1;

            //If beginning times are equal, compare end times.
            result = end?.CompareTo(other.end) ?? -1;
            if(result < 0) return -1;
            if(result > 0) return 1;

            //If end times are equeal, compare names.
            result = name?.CompareTo(other.name) ?? -1;
            return result;
        }
    }

    /// <summary>
    /// Represents a calendar event with all properties.
    /// </summary>
    public class CalendarEvent : CalendarEventBasic
    {
        public string? eventDescription { get; set; }
        public string? place {  get; set; }
        public CalendarEvent() { }
    }
}
