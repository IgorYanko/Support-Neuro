using System;

namespace NeuroApp
{

    public class User
    {
        public enum UserFunction
        {
            high = 3,
            mid = 2,
            low = 1,
            generic = 0
        }

        public string UserName { get; }
        public UserFunction Function { get; }

        public User(string username, string password, UserFunction function)
        { 
            UserName = username;
            Function = function;
        }
    }
}
