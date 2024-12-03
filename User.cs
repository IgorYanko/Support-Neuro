using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroApp
{
    public class User
    {
        public enum Function
        {
            adm = 4,
            fin = 3,
            eng = 2,
            sup = 1
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        public Function function { get; set; }
    }
}
