using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Constants
    {
        public const int SERVER_PORT = 10000;
        public const int SERVICE_PORT = 10001;

        public static string ValidTokenRequest { get; set; } = "Give me a token please";
        public static string InvalidTokenRequest { get; set; } = "Give me a token immediate";
        public static string Token { get; set; } = "SOMETOKEN123456";
    }
}
