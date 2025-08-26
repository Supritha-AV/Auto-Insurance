using Auto_Insurance_System.Data;
using Auto_Insurance_System.Interfaces;
using Auto_Insurance_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Auto_Insurance_System.Services
{
    public class ClaimService : IClaimService
    {
        private readonly AutoInsuranceDbContext context;
        public ClaimService(AutoInsuranceDbContext db)
        {
            context = db;
        }

        public List<Claim> GetAllClaims()
        {
            DbSet<Claim> claims = context.Claims;
            return claims.ToList();
        }

        public Claim GetClaimDetails(string id)
        {
            int claimId = Convert.ToInt32(id);
            DbSet<Claim> claims = context.Claims;
            Claim claim = claims.Find(claimId);
            return claim;
        }

        public bool SubmitClaim(Claim obj)
        {
            try
            {
                DbSet<Claim> claims = context.Claims;
                claims.Add(obj);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool UpdateClaimStatus(Claim obj)
        {
            try
            {
                DbSet<Claim> claims = context.Claims;
                Claim c = claims.Find(obj.ClaimId);
                if (c != null)
                {
                    c.PolicyId = obj.PolicyId;
                    c.ClaimStatus = obj.ClaimStatus;
                    c.ClaimAmount = obj.ClaimAmount;
                    c.ClaimDate = obj.ClaimDate;
                    c.AdjusterId = obj.AdjusterId;
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}