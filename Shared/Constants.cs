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

        public const string Admin = "Admin";
        public const string AdminPass = "norealpassword123_";

        public const string User = "User";
        public const string UserPass = "usernorealpassword123_";

        public const string ServiceName = "DemoService";
        public const string AuthService = "AuthService";

        public const string INVALIDTOKEN = "ERROR - invalid token";
        public const string TokenSignKey = "TWh5CVi0SSoo4CXc"; //this key is not real

        public const string ReadPermission = "READ_PERMISSION";
        public const string WritePermission = "WRITE_PERMISSION";
    }
}
