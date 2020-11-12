using System.Collections.Generic;
using CAcore.Models;

namespace CAcore.Data
{
    public interface ICAcoreRepo
    {
        bool SaveChanges();
        IEnumerable<User> GetAllUsers();
        User GetUserByUserId(string UserId);
        void CreateUser(User usr);
        void UpdateUser(User usr);
        void DeleteUser(User usr);

        UserCertificate CreateUserCertificate(string uid);

        void RevokeUserCertificate(string uid, string cid); 

        IEnumerable<UserCertificate> GetAllUserCertificates(string uid);

        UserCertificate GetUserCertificate(string uid, string cid);

    }
}