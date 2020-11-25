
using System; 

namespace CAwebapp.Models
{
    public class CertificatesStatus
    {
        public string NumberOfIssuedCertificates { get; set; }
        public string NumberOfRevokedCertificates { get; set; }
        public string CurrentSerialNumber { get; set; }
    }
}