using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroApp.Classes
{
    public class PermissionSystem
    {
        private static PermissionSystem _instance;

        public static PermissionSystem Instance => _instance ??= new PermissionSystem();

        private User _currentUser;

        public void SetCurrentUser(User user) 
        {
            _currentUser = user;
        }

        public User GetCurrentUser()
        { 
            return _currentUser; 
        }

        public bool HasPermission(User.Function requiredFunction)
        {
            if (_currentUser == null)
                throw new InvalidOperationException("Nenhum usuário autenticado.");

            return _currentUser.function >= requiredFunction;
        }
    }
}
