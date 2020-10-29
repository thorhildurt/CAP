using System.Collections.Generic;
using CAcore.Models;

namespace CAcore.Data
{
    public interface ICAcoreRepo
    {
        bool SaveChanges();
        IEnumerable<User> GetAllUsers();
        User GetUserByUserId(string UserId);
        void CreateUser(User usr);
    }
}