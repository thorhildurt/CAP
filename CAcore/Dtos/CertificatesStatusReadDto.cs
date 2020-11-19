
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAcore.Dtos
{
    public class CertificatesStatusReadDto
    {
        public string NumberOfIssuedCertificates { get; set; }
        public string NumberOfRevokedCertificates { get; set; }
        public string CurrentSerialNumber { get; set; }
    }
}