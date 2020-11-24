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
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using System.IO;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;
using System.Text.RegularExpressions;
using Org.BouncyCastle.OpenSsl;
using X509Extension = System.Security.Cryptography.X509Certificates.X509Extension;
using Org.BouncyCastle.X509.Extension;
using CAcore.Helpers;
using System.Text;
using Serilog;

namespace CAcore.Data
{
    public class MySqlCAcoreRepo : ICAcoreRepo
    {
        private readonly CAcoreContext _context;
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        private IConfiguration _configuration;

        private readonly UserHelper _userHelper;

        public MySqlCAcoreRepo(CAcoreContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _userHelper= new UserHelper();    
        }

        public IEnumerable<User> GetAllUsers()
        {   
            return _context.Users.ToList();
        }
        public User GetUserByUserId(string UserId)
        {   
            return _context.Users.FirstOrDefault(user => user.UserId == UserId);
        }

        public User GetUserByEmail(string Email)
        {   
            return _context.Users.FirstOrDefault(user => user.Email == Email);
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

        public void UpdateUser(User usr, string newPassword = "")
        {
            if (!String.IsNullOrEmpty(newPassword)) 
            {
                usr.Password = _userHelper.GetHashedPassword(newPassword);
            }

            _context.Users.Update(usr);
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
            if (user == null) 
            {
                return null; 
            }
            
            X509Certificate2 rootCert = getRootCert();
            if(rootCert == null) 
            {
                return null; 
            }

            if(!rootCert.HasPrivateKey) 
            {
                var privateKey = getRootPrivateKey();
                rootCert = rootCert.CopyWithPrivateKey(privateKey);
            }

            ECDsa userECDsa = ECDsa.Create();
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
                        new Oid("1.3.6.1.5.5.7.3.8"),
                        new Oid("1.3.6.1.5.5.7.3.2") // client authentication key usage
                    },
                    true));
            //adds an indentifier of public key to cert to make it easier to find
            req.CertificateExtensions.Add(
                new X509SubjectKeyIdentifierExtension(req.PublicKey, false));
            

            GeneralName uri = new GeneralName(GeneralName.UniformResourceIdentifier, _configuration["CrlDistPoint"]);
            DistributionPoint distPoint = new DistributionPoint(new DistributionPointName(DistributionPointName.FullName, uri), null, null);
            CrlDistPoint crlDistPoint = new CrlDistPoint(new DistributionPoint[] {distPoint});

            

            req.CertificateExtensions.Add(
                new X509Extension(
                    new Oid("2.5.29.31"),
                    crlDistPoint.GetDerEncoded(),
                    false
                )
            );

            // get the latest serial number
            var certs = GetAllCertificates();
            var latestCertificate = certs.OrderBy(x => x.SerialInDecimal).LastOrDefault();
            var currSerialNumber = 0;
            if (latestCertificate != null)
            {
                currSerialNumber = latestCertificate.SerialInDecimal;
            }
            // increment the latest serial number and used it for the new certificate
            int number = currSerialNumber + 1;
            byte [] intBytes = BitConverter.GetBytes(number);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(intBytes);
            }
            byte [] serialNumber = intBytes;
            
            // creating certificate signed with the root certificate
            X509Certificate2 cert =  req.Create(rootCert, DateTime.UtcNow, DateTime.UtcNow.AddDays(200), serialNumber); 
            cert = cert.CopyWithPrivateKey(userECDsa);
            Log.Information("Veriyfing newly issue cert...");
            _verify_certificate(cert);

            UserCertificate newCert = new UserCertificate 
            {
                UserId = uid,
                CertId = cert.SerialNumber, 
                SerialInDecimal = number,
                CertBodyPkcs12 = cert.Export(X509ContentType.Pkcs12, user.UserId),
                RawCertBody = cert.RawData,
                PrivateKey = cert.GetECDsaPrivateKey().ExportECPrivateKey()
            };
            _context.UserCertificates.Add(newCert);
            
            return newCert;
        }
 
        public void RevokeUserCertificate(string uid, string cid)
        {   
            //get root cert
            X509Certificate rootCert = DotNetUtilities.FromX509Certificate(getRootCert()); 
            
            //get user cert to be revoked
            UserCertificate userCert = GetUserCertificate(uid, cid);
            X509Certificate certToRevoke = new X509CertificateParser().ReadCertificate(userCert.RawCertBody);
            
            //parse the last CRL
            X509CrlParser crlParser = new X509CrlParser();
            FileStream fileStream = File.Open(_configuration["CrlPath"], FileMode.Open);
            X509Crl rootCrl =  crlParser.ReadCrl(fileStream);
            fileStream.Close();

            //extract the CRL number
            Asn1OctetString prevCrlNum = rootCrl.GetExtensionValue(X509Extensions.CrlNumber);
            Asn1Object obj = X509ExtensionUtilities.FromExtensionValue(prevCrlNum);
            BigInteger prevCrlNumVal = DerInteger.GetInstance(obj).PositiveValue;

            //generate new CRL
            X509V2CrlGenerator crlGenerator = new X509V2CrlGenerator();
            crlGenerator.SetIssuerDN(rootCert.SubjectDN);
            crlGenerator.SetThisUpdate(DateTime.UtcNow);
            crlGenerator.SetNextUpdate(DateTime.UtcNow.AddDays(10));
            crlGenerator.AddCrl(rootCrl); //add the old CRL entries
            //add the newly revoked certificates
            crlGenerator.AddCrlEntry(certToRevoke.SerialNumber, DateTime.UtcNow, CrlReason.PrivilegeWithdrawn);
            //increment CRL Number by 1
            crlGenerator.AddExtension("2.5.29.20", false, new CrlNumber(prevCrlNumVal.Add(BigInteger.One)));
            AsymmetricKeyParameter bouncyCastlePrivateKey =  PrivateKeyFactory.CreateKey(getRootPrivateKey().ExportPkcs8PrivateKey());

            var sigFactory = new Asn1SignatureFactory("SHA256WITHECDSA", bouncyCastlePrivateKey);
            X509Crl nextCrl = crlGenerator.Generate(sigFactory);
            
            // writePem(_configuration["CrlOldPath"], rootCrl); // write old CRL as backup
            writePem(_configuration["CrlPath"], nextCrl); //write new CRL
            
            // sanity check
            nextCrl.Verify(rootCert.GetPublicKey());
            
            userCert.Revoked = true;
            _context.UserCertificates.Update(userCert);
        }

        public IEnumerable<UserCertificate> GetAllUserCertificates(string uid)
        {
            return _context.UserCertificates.Where(cert => cert.UserId == uid);
        }

        public IEnumerable<UserCertificate> GetAllCertificates()
        {
            return _context.UserCertificates.ToList();
        }

        public UserCertificate GetUserCertificate(string uid, string cid)
        {
            UserCertificate cert =  _context.UserCertificates.FirstOrDefault(cert => cert.CertId == cid && cert.UserId == uid);

            if (cert == null)
            {
                return cert;
            }

            X509Certificate2 xCert = new X509Certificate2(cert.RawCertBody);
            Log.Information("Verifying obtained cert...");
            _verify_certificate(xCert);
            return cert; 
        }

        private ECDsa getRootPrivateKey() 
        {
            string keyFile= File.ReadAllText(_configuration["PrivateKeyPath"]);
            // extract base64 encoded private key
            Regex regex = new Regex(@"(-----BEGIN ENCRYPTED PRIVATE KEY-----)((.|\n)*)(-----END ENCRYPTED PRIVATE KEY-----)");
            string keyBase64 = regex.Match(keyFile).Groups[2].ToString();
            
            ECDsa eC = ECDsa.Create();
            string pw = _configuration["PrivateKeyPw"];
            // byte[] pwBytes = Encoding.UTF8.GetBytes(pw);
            eC.ImportEncryptedPkcs8PrivateKey(Encoding.UTF8.GetBytes(pw), Convert.FromBase64String(keyBase64), out _);
            return eC; 
        }

        private void writePem(string filename, object obj) 
        {
            PemWriter pemWriter = new PemWriter(new StreamWriter(File.Open(filename, FileMode.Create)));
            pemWriter.WriteObject(obj);
            pemWriter.Writer.Flush();
            pemWriter.Writer.Close();
        }

        private X509Certificate2 getRootCert() 
        {
            X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.OpenExistingOnly);
            X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
            X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindByThumbprint, _configuration["CertThumbprint"], false);
            if(fcollection.Count == 0) 
            {
                Log.Fatal("No root certificate found.");
                return null; 
            }

            X509Certificate2 rootCert = fcollection[0];

            Log.Information("Root cert verify " + rootCert.Verify());
            _verify_certificate(rootCert);
            // store.Close();
            return rootCert; 
        }

        private void _verify_certificate(X509Certificate2 cert) 
        {
            X509Chain chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            // temporary just to bypass unknown revocation status error
            // chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
            try
            {
                var chainBuilt = chain.Build(cert);
                Log.Information(string.Format("Chain building status: {0}", chainBuilt));

                if (chainBuilt == false)
                {
                    foreach (X509ChainStatus chainStatus in chain.ChainStatus)
                    {
                        Log.Fatal(string.Format("Chain error: {0} {1}", chainStatus.Status, chainStatus.StatusInformation));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.ToString());
            }
        }
    }
}