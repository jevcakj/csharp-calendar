using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarCommon
{
    public class CalendarEvent
    {
        public int? id {  get; set; }
        public string? name { get; set; }
        public DateTime? dateTime { get; set; }
        public string? description {  get; set; }

        public CalendarEvent() { }

        public CalendarEvent(int id, string name, DateTime dateTime)
        {
            this.id = id;
            this.name = name;
            this.dateTime = dateTime;
        }
        
        public CalendarEvent(int id, string name, DateTime dateTime, string description) {
            this.id = id;
            this.name = name;
            this.dateTime = dateTime;
            this.description = description;
        }
    }
}
