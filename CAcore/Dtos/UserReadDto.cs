using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CAcore.Dtos
{
    public class UserReadDto
    {
        public string UserId { get; set; } 

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
    }
}