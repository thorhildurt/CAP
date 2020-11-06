// using System;
// using System.Collections.Generic;
// using System.Security.Cryptography;
// using System.Security.Cryptography.X509Certificates;
// using CAcore.Models;

// namespace CAcore.Data
// {
//     public class MockCAcoreRepo : ICAcoreRepo
//     {
//         List<UserCertificate> certs = new List<UserCertificate>(); 
//         ECDsa ecdsa;
//         X509Certificate2 mockCert; 
//         public MockCAcoreRepo() {
//             ecdsa = ECDsa.Create(); 
//             CertificateRequest req = new CertificateRequest("CN=IMovies CA", ecdsa, HashAlgorithmName.SHA256);  
//             req.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
//             req.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(req.PublicKey, false));
//             mockCert = req.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-2), DateTimeOffset.UtcNow.AddDays(200));
//             Console.WriteLine(mockCert.Verify());
//         }
//         public void DeleteUser(User usr)
//         {
//             throw new System.NotImplementedException();
//         }
//         public void UpdateUser(User usr)
//         {
//             throw new System.NotImplementedException();
//         }
//         public void CreateUser(User usr)
//         {
//             throw new System.NotImplementedException();
//         }

//         public bool SaveChanges()
//         {
//             throw new System.NotImplementedException();
//         }

//         public IEnumerable<User> GetAllUsers()
//         {
//             var users = new List<User>
//             {
//                 new User { UserId="1", FirstName="John", LastName="Johnsson", Email="jj@imovie.com", Password="pw1" },
//                 new User { UserId="2", FirstName="Tim", LastName="Scott", Email="ts@imovie.com", Password="pw2" },
//                 new User { UserId="3", FirstName="Susan", LastName="Clark", Email="sc@imovie.com", Password="pw3" }
//             };

//             return users;
//         }
//         public User GetUserByUserId(string UserId)
//         {
//             return new User { UserId="1", FirstName="John", LastName="Johnsson", Email="jj@imovie.com", Password="pw" };
//         }
//     }
// }