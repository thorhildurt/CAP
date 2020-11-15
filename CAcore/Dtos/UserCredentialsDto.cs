using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CAcore.Dtos
{
    public class UserCredentialsDto
    {
        [Required]
        public string UserId { get; set; } 

        [Required]
        public string Password { get; set; }
    }
}