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
        public string? Name { get; set; }
        public string? Password { get; set; }

        public User() { }

        public User(int id, string name, string password)
        {
            this.id = id;
            this.Name = name;
            this.Password = password;
        }
    }
}
