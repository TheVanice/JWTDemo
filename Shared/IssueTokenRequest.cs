using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class IssueTokenRequest
    {
        public string Service { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Pass { get; set; } = string.Empty;

        public static IssueTokenRequest FromString(string message)
        {
            var parts = message.Split(';');
            return new IssueTokenRequest
            {
                UserName = parts[0],
                Pass = parts[1],
                Service = parts[2],
            };
        }

        public override string ToString()
        {
            return $"{UserName};{Pass};{Service}";
        }
    }
}
