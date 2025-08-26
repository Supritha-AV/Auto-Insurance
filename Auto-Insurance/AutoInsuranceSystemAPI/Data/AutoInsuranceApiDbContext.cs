using AutoInsuranceSystemAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoInsuranceSystemAPI.Data
{
    public class AutoInsuranceApiDbContext : DbContext
    {
        public AutoInsuranceApiDbContext(DbContextOptions<AutoInsuranceApiDbContext> options) : base(options) { }

        public virtual DbSet<Policy> Policy { get; set; }
    }
} 