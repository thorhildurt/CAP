using System.Collections.Generic;
using CAcore.Models;

namespace CAcore.Data
{
    public interface ICAcoreRepo
    {
        IEnumerable<User> GetAllUsers();
        User GetUserByUserId(string UserId);
    }
}