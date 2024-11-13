using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroApp
{
    public class User
    {
        public enum PermissionLevel
        {
            low = 1,
            medium = 2,
            high = 3
        }

        public string userName { get; set; }
        public string Password { get; set; }
        public PermissionLevel permissionLevel { get; set; }
    }
}
