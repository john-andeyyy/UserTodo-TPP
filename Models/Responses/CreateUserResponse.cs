using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notification.Models.Responses
{
    public class CreateUserResponse
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
    }
}
