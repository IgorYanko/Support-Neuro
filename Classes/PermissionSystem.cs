using System;

namespace NeuroApp.Classes
{
    public class PermissionSystem
    {
        private static readonly Lazy<PermissionSystem> _instance = new(() => new PermissionSystem());
        public static PermissionSystem Instance => _instance.Value;

        private User _currentUser;
        private readonly object _lock = new();

        private PermissionSystem() { }

        public void SetCurrentUser(User user) 
        {
            if (user == null) throw new ArgumentNullException(nameof(user), "Usuário não pode ser nulo.");

            lock (_lock)
            {
                _currentUser = user;
            }
        }

        public User GetCurrentUser()
        {
            if (_currentUser == null)
                throw new InvalidOperationException("Nenhum usuário foi definido no sistema de permissões.");
            
            return _currentUser; 
        }

        public bool HasPermission(User.UserFunction requiredFunction) => _currentUser?.Function >= requiredFunction;
    }
}
