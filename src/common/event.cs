using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarCommon
{
    public class CalendarEventBasic
    {
        public int? id {  get; set; }
        public string? name { get; set; }
        public DateTime? beginning { get; set; }
        public DateTime? end {  get; set; }

        public CalendarEventBasic() { }
    }
    public class CalendarEvent : CalendarEventBasic
    {
        public string? eventDescription { get; set; }
        public string? place {  get; set; }
        public CalendarEvent() { }
    }
}
