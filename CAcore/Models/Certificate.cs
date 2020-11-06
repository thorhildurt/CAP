
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
        [Column("certBody")] // X509Certificate2.RawData
        public byte[] CertBody { get; set; }

        [Required]
        [Column("privateKey")]
        public byte[] PrivateKey { get; set; }
    }
}