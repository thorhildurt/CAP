// using System;
// using System.Collections.Generic;
// using System.Security.Cryptography;
// using System.Security.Cryptography.X509Certificates;
// using CAcore.Models;

// namespace CAcore.Data {
//     public class MockCertificateRepo : ICertificateRepo {
//         List<UserCertificate> certs = new List<UserCertificate>(); 
//         ECDsa ecdsa;
//         X509Certificate2 mockCert; 
//         public  MockCertificateRepo() {
//             ecdsa = new ECDsaCng();
//             CertificateRequest req = new CertificateRequest("CN=IMovies CA", ecdsa, HashAlgorithmName.SHA256);  
//             req.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
//             req.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(req.PublicKey, false));
//             mockCert = req.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-2), DateTimeOffset.UtcNow.AddDays(200));
//             Console.WriteLine(mockCert.Verify());
//         }
//         public void CreateCertificate(string uid)
//         {   
//             //initializing ECDsa object
//             ECDsaCng userECDsa = new ECDsaCng();
//             // initializing certificate request 
//             string name = "CN=User" + uid; 
//             Console.WriteLine("YOOOOOO " + name);
//             CertificateRequest req = new CertificateRequest(name, userECDsa, HashAlgorithmName.SHA256); 
//             req.CertificateExtensions.Add(
//             new X509BasicConstraintsExtension(false, false, 0, false));

//             req.CertificateExtensions.Add(
//                 new X509KeyUsageExtension(
//                     X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation,
//                     false));

//             req.CertificateExtensions.Add(
//                 new X509EnhancedKeyUsageExtension(
//                     new OidCollection
//                     {
//                         new Oid("1.3.6.1.5.5.7.3.8")
//                     },
//                     true));

//             req.CertificateExtensions.Add(
//                 new X509SubjectKeyIdentifierExtension(req.PublicKey, false));


//             // creating certificate signed with the mock root certificate
            
//             X509Certificate2 cert =  req.Create(mockCert, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(200), new byte[]{1, 2, 3}); 
//             cert = cert.CopyWithPrivateKey(userECDsa);
//             Console.WriteLine(cert.Verify());
            

//             // serializing and encrypting the private key to bytes
           
//             byte [] privKeyBytes = cert.GetECDsaPrivateKey().ExportEncryptedPkcs8PrivateKey(new ReadOnlySpan<Byte>(new byte[]{1, 2, 3}),
//                 new PbeParameters(PbeEncryptionAlgorithm.Aes128Cbc, HashAlgorithmName.SHA256, 12));
            
            
            
//             // PKCS12 representation of the issued certificate
//             byte[] certBody = cert.Export(X509ContentType.Pkcs12);

//             // creating UserCertificate object (our internal representation) and adding it to the list of certs
//             UserCertificate userCert = new UserCertificate {certBody= certBody, privateKey = privKeyBytes, UserId = uid, CertId = "1"};
//             Console.WriteLine(userCert);
//             certs.Add(userCert);
//         }

//         public IEnumerable<UserCertificate> GetAllCertificates()
//         {
//             return certs; 
//         }

//         public IEnumerable<UserCertificate> GetAllUserCertificates(string uid)
//         {
//             List<UserCertificate> res = certs.FindAll(c => c.UserId.Equals(uid)); 

//             return res; 
//         }

//         public UserCertificate GetUserCertificate(string cid)
//         {
//             return certs.Find(c => c.CertId.Equals(cid)); 
//         }

//         public void RevokeCertificate(string cid)
//         {
//             throw new System.NotImplementedException();
//         }
//     }
// }