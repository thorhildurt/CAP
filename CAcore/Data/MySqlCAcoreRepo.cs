
using System.Collections.Generic;
using System.Linq;
using CAcore.Models;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CAcore.Data
{
    public class MySqlCAcoreRepo : ICAcoreRepo
    {
        private readonly CAcoreContext _context;
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        public MySqlCAcoreRepo(CAcoreContext context)
        {
            _context = context;
        }
        public IEnumerable<User> GetAllUsers()
        {   
            return _context.Users.ToList();
        }
        public User GetUserByUserId(string UserId)
        {   
            return _context.Users.FirstOrDefault(user => user.UserId == UserId);
        }

        public void CreateUser(User usr)
        {
            if (usr == null)
            {
                throw new ArgumentNullException(nameof(usr));
            }
            if (_context.Users.Where(user => user.Email == usr.Email).FirstOrDefault() != null)
            {
                throw new ArgumentException(String.Format("{0}- The email address {1} exists", nameof(usr), usr.Email));
            }
            _context.Users.Add(usr);
        }

        public void UpdateUser(User usr)
        {
            // nothing
        }

        public void DeleteUser(User usr)
        {
            if (usr == null)
            {
                throw new ArgumentNullException(nameof(usr));
            }
            _context.Users.Remove(usr);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }



        public UserCertificate CreateUserCertificate(string uid)
        {
            User user = GetUserByUserId(uid);
            if (user == null) {
                return null; 
            }
            X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            store.Open(OpenFlags.OpenExistingOnly);
            X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
            X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindByIssuerName, "IMovies Core CA", true);
            Console.WriteLine(store.Certificates.Count);
            if(fcollection.Count == 0) {
                Console.WriteLine("No cert found");
                return null; 
            }

            X509Certificate2 rootCert = fcollection[0];

            Console.WriteLine("Root cert verify " + rootCert.Verify());

              ECDsaCng userECDsa = new ECDsaCng();
            // initializing certificate request 
            CertificateRequest req = new CertificateRequest($"CN={user.FirstName} {user.LastName}", userECDsa, HashAlgorithmName.SHA256); 
            req.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(false, false, 0, false));

            req.CertificateExtensions.Add(
                new X509KeyUsageExtension(
                    X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation,
                    false));

            // Binding the hash of an object to a time
            req.CertificateExtensions.Add(
                new X509EnhancedKeyUsageExtension(
                    new OidCollection
                    {
                        new Oid("1.3.6.1.5.5.7.3.8")
                    },
                    true));
            //adds an indentifier of public key to cert to make it easier to find
            req.CertificateExtensions.Add(
                new X509SubjectKeyIdentifierExtension(req.PublicKey, false));


            // creating certificate signed with the root certificate
            byte [] serialNumber = new byte[20];
            rng.GetBytes(serialNumber);
            //problem is here -- probably because private key of cert is encrypted
            X509Certificate2 cert =  req.Create(rootCert, DateTime.UtcNow, DateTime.UtcNow.AddDays(200), serialNumber); 
            cert = cert.CopyWithPrivateKey(userECDsa);
            Console.WriteLine(cert.Verify());
            Console.WriteLine(cert.GetECDsaPublicKey().Equals(cert.GetECDsaPrivateKey()));
            

            UserCertificate newCert =  new UserCertificate {
                UserId = uid,
                CertId = serialNumber.ToString(), 
                CertBody = cert.RawData,
                PrivateKey = cert.GetECDsaPrivateKey().ExportECPrivateKey()
                };
            _context.UserCertificates.Add(newCert);
            
            return newCert; 
        }

        public void RevokeUserCertificate(UserCertificate cert)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserCertificate> GetAllUserCertificates(string uid)
        {
            throw new NotImplementedException();
        }

        public UserCertificate GetUserCertificate(string uid, string cid)
        {
            throw new NotImplementedException();
        }
    }
}