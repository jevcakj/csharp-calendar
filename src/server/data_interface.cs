﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using CalendarCommon;

namespace CalendarServer
{
    public interface IData
    {
        public bool CreateUser(User user);
        public bool AuthenticateUser(User user);
        public void SaveEvent(CalendarEvent e);
        public void DeleteEvent(DateTime dateTime, int id);
        //?
        public List<CalendarEvent> GetEvents(Expression e);
    }
}
