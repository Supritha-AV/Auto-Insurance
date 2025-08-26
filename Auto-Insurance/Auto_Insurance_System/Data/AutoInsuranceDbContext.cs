using Auto_Insurance_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Auto_Insurance_System.Data
{
    public class AutoInsuranceDbContext : DbContext
    {
        public AutoInsuranceDbContext(DbContextOptions<AutoInsuranceDbContext> options) : base(options) { }
        public virtual DbSet<Policy> Policy { get; set; }
        public virtual DbSet<Claim> Claims { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<SupportTicket> SupportTickects { get; set; }
        public virtual DbSet<Users> Users { get; set; }
    }
}
