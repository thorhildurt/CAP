// using System;
// using System.Collections.Generic;
// using System.Security.Cryptography;
// using System.Security.Cryptography.X509Certificates;
// using CAcore.Models;


using System.Collections.Generic;
using System.Linq;
using CAcore.Models;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CAcore.Dtos;
using AutoMapper.Configuration;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto;
using System.IO;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Prng;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;
using Org.BouncyCastle.Asn1.Pkcs;
using System.Text.RegularExpressions;
using Org.BouncyCastle.OpenSsl;
using System.Text;
using X509Extension = System.Security.Cryptography.X509Certificates.X509Extension;

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
                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.OpenExistingOnly);
                X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
                X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindByThumbprint, Startup.CertThumbprint, false);
                Console.WriteLine(collection[0].IssuerName.Name);
                if(fcollection.Count == 0) {
                    Console.WriteLine("No cert found");
                    return null; 
                }

                X509Certificate2 rootCert = fcollection[0];

                Console.WriteLine("Root cert verify " + rootCert.Verify());
                _verify_certificate(rootCert);
                ECDsaCng userECDsa = new ECDsaCng();
                // initializing certificate request 
                CertificateRequest req = new CertificateRequest($"CN={user.FirstName} {user.LastName}, E={user.Email}", userECDsa, HashAlgorithmName.SHA256); 
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

                req.CertificateExtensions.Add(
                    new X509Extension(
                        new Oid("2.5.29.31"),
                        Encoding.ASCII.GetBytes("http://localhost:5000/root.crl"),
                        false
                    )
                );

                // creating certificate signed with the root certificate
                byte [] serialNumber = new byte[20];
                rng.GetBytes(serialNumber);
                X509Certificate2 cert =  req.Create(rootCert, DateTime.UtcNow, DateTime.UtcNow.AddDays(200), serialNumber); 
                cert = cert.CopyWithPrivateKey(userECDsa);
                File.WriteAllBytes("test.pfx", cert.Export(X509ContentType.Pfx));
                Console.WriteLine(cert.ToString());
                foreach(X509Extension ext in cert.Extensions) {
                    Console.WriteLine(ext.Format(true));
                }
                cert.Verify();
                _verify_certificate(cert);
                Console.WriteLine(cert.GetECDsaPublicKey().Equals(cert.GetECDsaPrivateKey()));
                
                
                UserCertificate newCert =  new UserCertificate {
                    UserId = uid,
                    CertId = cert.SerialNumber.ToString(), 
                    CertBodyPkcs12 = cert.Export(X509ContentType.Pkcs12),
                    RawCertBody = cert.RawData,
                    PrivateKey = cert.GetECDsaPrivateKey().ExportECPrivateKey()
                    };
                _context.UserCertificates.Add(newCert);
                
                return newCert;
            }

            private void _verify_certificate(X509Certificate2 cert) {
                X509Chain chain = new X509Chain();

                try
                {
                    var chainBuilt = chain.Build(cert);
                    Console.WriteLine(string.Format("Chain building status: {0}", chainBuilt));

                    if (chainBuilt == false)
                        foreach (X509ChainStatus chainStatus in chain.ChainStatus)
                            Console.WriteLine(string.Format("Chain error: {0} {1}", chainStatus.Status, chainStatus.StatusInformation));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
        }
 
        public void RevokeUserCertificate(string uid, string cid)
        {   
            //get root cert
            X509Certificate rootCert = DotNetUtilities.FromX509Certificate(getRootCert()); 
            
            
            //get user cert to be revoked
            UserCertificate userCert = GetUserCertificate(uid, cid);
            X509Certificate certToRevoke = new X509CertificateParser().ReadCertificate(userCert.RawCertBody);

            X509CrlParser crlParser = new X509CrlParser();
            FileStream fileStream = File.Open("Content\\root.crl", FileMode.Open);
            X509Crl rootCrl =  crlParser.ReadCrl(fileStream);
            fileStream.Close();
            Asn1OctetString prevCrlNum = rootCrl.GetExtensionValue(X509Extensions.CrlNumber);
            X509V2CrlGenerator crlGenerator = new X509V2CrlGenerator();
            crlGenerator.SetIssuerDN(rootCert.SubjectDN);
            crlGenerator.SetThisUpdate(DateTime.UtcNow);
            crlGenerator.SetNextUpdate(DateTime.UtcNow.AddYears(1));
            crlGenerator.AddCrl(rootCrl); //add the old CRL
            crlGenerator.AddCrlEntry(certToRevoke.SerialNumber, DateTime.UtcNow, CrlReason.PrivilegeWithdrawn);
            CrlNumber nextCrlNum = new CrlNumber(BigInteger.Three);
            crlGenerator.AddExtension("2.5.29.20", false, nextCrlNum);
            
            string keyFile= File.ReadAllText("C:\\Users\\radwa\\core_ca\\test\\ca.key");

            Regex regex = new Regex(@"(-----BEGIN EC PRIVATE KEY-----)((.|\n)*)(-----END EC PRIVATE KEY-----)");
            string keyBase64 = regex.Match(keyFile).Groups[2].ToString();
            
            ECDsa eC = ECDsa.Create();
            eC.ImportECPrivateKey(Convert.FromBase64String(keyBase64), out _);
            
            AsymmetricKeyParameter bouncyCastlePrivateKey =  PrivateKeyFactory.CreateKey(eC.ExportPkcs8PrivateKey());
            
            var sigFactory = new Asn1SignatureFactory("SHA256WITHECDSA", bouncyCastlePrivateKey);
            X509Crl nextCrl = crlGenerator.Generate(sigFactory);
            writePem("Content\\root.crl.old", rootCrl);
            writePem("Content\\root.crl", nextCrl);

            // sanity check
            nextCrl.Verify(rootCert.GetPublicKey());

        }

        private void writePem(string filename, object obj) {
            PemWriter pemWriter = new PemWriter(new StreamWriter(File.Open(filename, FileMode.Create)));
            pemWriter.WriteObject(obj);
            pemWriter.Writer.Flush();
            pemWriter.Writer.Close();
        }

        private X509Certificate2 getRootCert() {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.OpenExistingOnly);
                X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
                X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindByThumbprint, Startup.CertThumbprint, false);
                Console.WriteLine(collection[0].IssuerName.Name);
                if(fcollection.Count == 0) {
                    Console.WriteLine("No cert found");
                    return null; 
                }

                X509Certificate2 rootCert = fcollection[0];

                Console.WriteLine("Root cert verify " + rootCert.Verify());
                _verify_certificate(rootCert);

                return rootCert; 
        }
        public IEnumerable<UserCertificate> GetAllUserCertificates(string uid)
        {
            return _context.UserCertificates.Where(cert => cert.UserId == uid);
        }

        public UserCertificate GetUserCertificate(string uid, string cid)
        {
            return _context.UserCertificates.FirstOrDefault(cert => cert.CertId == cid && cert.UserId == uid);
        }
    }
}