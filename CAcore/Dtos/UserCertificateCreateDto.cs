
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAcore.Dtos
{
    
    public class UserCertificateCreateDto
    {
    

        [Required]
        [Column("uid")]
        public string UserId { get; set; }

    }
}