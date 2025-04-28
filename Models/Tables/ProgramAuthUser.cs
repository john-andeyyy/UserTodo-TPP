using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notification.Models.Tables
{
    public class ProgramAuthUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Key { get; set; }

        internal ProgramAuthUser WithoutPassword()
        {
            return this;
        }
    }
}
