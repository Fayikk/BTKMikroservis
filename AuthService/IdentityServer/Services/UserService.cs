using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Models;

namespace IdentityServer.Services
{
    public class UserService
    {
         private readonly List<User> _users;
            public UserService()
        {
            _users = new List<User>
            {
                new User { Id = 1, Username = "admin", Password = "admin123", Role = "Admin", Email = "admin@demo.com" },
                new User { Id = 2, Username = "user", Password = "user123", Role = "User", Email = "user@demo.com" },
                new User { Id = 3, Username = "manager", Password = "manager123", Role = "Manager", Email = "manager@demo.com" }
            };
        }


          public User? ValidateUser(string username, string password)
        {
            return _users.FirstOrDefault(u =>
                u.Username == username && u.Password == password);
        }

    }
}