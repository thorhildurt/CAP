using System.Collections.Generic;
using CAcore.Models;
using System;

namespace CAcore.Data
{
    public interface ICAcoreRepo
    {
        bool SaveChanges();
        IEnumerable<User> GetAllUsers();
        User GetUserByUserId(string UserId);
        User GetUserByEmail(string Email);
        void CreateUser(User usr);
        void UpdateUser(User usr, string newPassword = "");
        void DeleteUser(User usr);
    }
}