
using System.Collections.Generic;
using System.Linq;
using CAcore.Models;
using System;
using System.Security.Cryptography;
using System.Text;

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
        public User GetUserByUserId(string UserId)
        {   
            return _context.Users.FirstOrDefault(user => user.UserId == UserId);
        }

        public void CreateUser(User usr)
        {
            if (usr == null)
            {
                throw new ArgumentNullException(nameof(usr));
            }
            if (_context.Users.Where(user => user.Email == usr.Email) != null)
            {
                throw new ArgumentException(String.Format("{0} - The email address {1} exists", nameof(usr), usr.Email));
            }
            _context.Users.Add(usr);
        }

        public void UpdateUser(User usr)
        {
            if (usr.NewPassword != null) 
            {
                var sha1 = SHA1.Create();
                var hash = sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(usr.NewPassword));
                var sBuilder = new StringBuilder();

                for (int i = 0; i < hash.Length; i++)
                {
                    sBuilder.Append(hash[i].ToString("x2"));
                }
                usr.Password = sBuilder.ToString();
            }

            _context.Users.Update(usr);
        }

        public void DeleteUser(User usr)
        {
            if (usr == null)
            {
                throw new ArgumentNullException(nameof(usr));
            }
            _context.Users.Remove(usr);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}