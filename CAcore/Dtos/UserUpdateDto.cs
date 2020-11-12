using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CAcore.Dtos
{
    public class UserUpdateDto
    {   
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        
        public string Password { get; set; }

        public string NewPassword { get; set; }
    }
}