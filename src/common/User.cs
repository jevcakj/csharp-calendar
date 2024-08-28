using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarCommon
{
    public class User
    {
        public int? id {  get; set; }
        public string? name { get; set; }
        public string? password { get; set; }

        public User() { }

        public User(int id, string name, string password)
        {
            this.id = id;
            this.name = name;
            this.password = password;
        }
    }
}
