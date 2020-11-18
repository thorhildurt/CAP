
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAcore.Models 
{
    public class UserCertificate 
    {
        [Key]
        [Column("cid")]
        public string CertId { get; set; }

        [Required]
        [Column("uid")]
        public string UserId { get; set; }

        [Required]
        [Column("rawCertBody")] // X509Certificate2.RawData
        public byte[] RawCertBody { get; set; }

        [Required]
        [Column("certBodyPkcs12")]
        public byte[] CertBodyPkcs12 { get; set; }

        [Required]
        [Column("privateKey")]
        public byte[] PrivateKey { get; set; }

        [Required]
        [Column("revoked")]
        public bool Revoked { get; set; }
    }
}