using System.Collections.Generic;
using CAcore.Models;

namespace CAcore.Data
{
    public class MockCAcoreRepo : ICAcoreRepo
    {
        public IEnumerable<User> GetAllUsers()
        {
            var users = new List<User>
            {
                new User { userId="1", FirstName="John", LastName="Johnsson", Email="jj@imovie.com", Password="pw1" },
                new User { userId="2", FirstName="Tim", LastName="Scott", Email="ts@imovie.com", Password="pw2" },
                new User { userId="3", FirstName="Susan", LastName="Clark", Email="sc@imovie.com", Password="pw3" }
            };

            return users;
        }
        public User GetUserById(int id)
        {
            return new User { userId="1", FirstName="John", LastName="Johnsson", Email="jj@imovie.com", Password="pw" };
        }
    }
}