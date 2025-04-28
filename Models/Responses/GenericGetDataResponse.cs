using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Notification.Models.Responses
{
    public class GenericGetDataResponse
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        //public DataTable Data { get; set; }
        public List<Dictionary<string, string>> Data { get; set; }
    }

    public class genericResponseV2
    {
        public string message { get; set; }
    }

    public class genericResponseWithDataV2
    {
        public string message { get; set; }
        public object data { get; set; }
    }
}
