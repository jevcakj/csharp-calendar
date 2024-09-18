using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarCommon
{
    public class CalendarEventBasic : IComparable<CalendarEventBasic>
    {
        public int? id {  get; set; }
        public string? name { get; set; }
        public DateTime? beginning { get; set; }
        public DateTime? end {  get; set; }

        public CalendarEventBasic() { }

        public bool IsValid() => id != null && name != null && beginning != null && end != null;

        public int CompareTo(CalendarEventBasic? other)
        {
            if (other is null) return 1;

            int result = beginning?.CompareTo(other.beginning) ?? -1;
            if(result < 0) return -1;
            if(result > 0) return 1;

            result = end?.CompareTo(other.end) ?? -1;
            if(result < 0) return -1;
            if(result > 0) return 1;

            result = name?.CompareTo(other.name) ?? -1;
            return result;
        }
    }

    public class CalendarEvent : CalendarEventBasic
    {
        public string? eventDescription { get; set; }
        public string? place {  get; set; }
        public CalendarEvent() { }
    }
}
