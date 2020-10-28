
using System.Collections.Generic;
using System.Linq;
using CAcore.Models;

namespace CAcore.Data
{
    public class MySqlCAcoreRepo : ICAcoreRepo
    {
        private readonly CAcoreContext _context;
        public MySqlCAcoreRepo(CAcoreContext context)
        {
            _context = context;
        }
        public IEnumerable<User> GetAllUsers()
        {   
            return _context.Users.ToList();
        }
        public User GetUserById(string UserId)
        {   
            return _context.Users.FirstOrDefault(user => user.UserId == UserId);
        }
    }
}