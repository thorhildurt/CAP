using System;

namespace CAwebapp.Models
{
    public class UserUpdate
    {   
        public string UserId { get; set; } 

        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public string Password { get; set; }

        public string NewPassword { get; set; }
    }
}