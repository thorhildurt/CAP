
using System.Collections.Generic;
using CAcore.Models; 

namespace CAcore.Data {

    public interface ICertificateRepo {

        IEnumerable<UserCertificate> GetAllCertificates();

        IEnumerable<UserCertificate> GetAllUserCertificates(string uid); 

        void CreateCertificate(string uid); 

        void RevokeCertificate(string cid); 

        UserCertificate GetUserCertificate(string cid);   
    }
}