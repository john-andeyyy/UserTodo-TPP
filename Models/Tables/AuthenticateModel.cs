using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Notification.Models.Tables
{
    public class AuthenticateModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class AuthenticateModelV2
    {
        [Required]
        public string username { get; set; }

        [Required]
        public string password { get; set; }
    }
}
