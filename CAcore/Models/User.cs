using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAcore.Models
{
    public class User
    {
        [Key]
        [Column("uid")]
        public string UserId { get; set; } 
        [Required]
        [Column("firstname")]
        public string FirstName { get; set; }
        [Required]
        [Column("lastname")]
        public string LastName { get; set; }
        [Required]
        [Column("email")]
        public string Email { get; set; }
        [Required]
        [Column("pwd")]
        public string Password { get; set; }
    }
}