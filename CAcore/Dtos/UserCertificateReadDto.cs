
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAcore.Dtos
{
    
    public class UserCertificateReadDto
    {
        [Key]
        [Column("cid")]
        public string CertId { get; set; }

        [Required]
        [Column("uid")]
        public string UserId { get; set; }

        [Required]
        [Column("certBody")] 
        public byte[] CertBodyPkcs12 { get; set; }

        
    }
}