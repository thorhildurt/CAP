
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAwebapp.Models
{
    public class UserCertificate
    {
        [Key]
        public string CertId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public byte[] CertBodyPkcs12 { get; set; }

        [Required]
        public byte[] PrivateKey { get; set; }

        [Required]
        public bool Revoked { get; set; }
    }
}