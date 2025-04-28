using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TPP.Models.Requests
{
    public class ProviderDetail
    {
        public string ProviderCode { get; set; }
        public string ProviderName { get; set; }
        public string ProviderDescritpiion { get; set; }
        public List<Dictionary<string, string>> Credentials { get; set; }
    }
}
