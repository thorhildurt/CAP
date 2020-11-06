using CAcore.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CAcore.Data
{
    public class CAcoreContext : DbContext
    {
        public CAcoreContext(DbContextOptions<CAcoreContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserCertificate> UserCertificates { get; set; }
    }
}