using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Notification.Models.Tables;

namespace Notification.Models.Responses
{
    public class GetUserListResponse
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public List<User> UsersList { get; set; }

    }
}
