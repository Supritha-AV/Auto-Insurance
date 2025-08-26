using AutoInsuranceSystemAPI.Data;
using AutoInsuranceSystemAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoInsuranceSystemAPI.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly AutoInsuranceApiDbContext dbContext;
        public PolicyService(AutoInsuranceApiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Policy>> GetAllAsync()
        {
            return await dbContext.Policy.AsNoTracking().ToListAsync();
        }

        public async Task<Policy?> GetByIdAsync(int id)
        {
            return await dbContext.Policy.AsNoTracking().FirstOrDefaultAsync(p => p.PolicyId == id);
        }

        public async Task<Policy> CreateAsync(Policy policy)
        {
            policy.PolicyId = 0;
            dbContext.Policy.Add(policy);
            await dbContext.SaveChangesAsync();
            return policy;
        }

        public async Task<bool> UpdateAsync(int id, Policy policy)
        {
            var existing = await dbContext.Policy.FirstOrDefaultAsync(p => p.PolicyId == id);
            if (existing == null) return false;

            existing.PolicyNumber = policy.PolicyNumber;
            existing.VehicleDetails = policy.VehicleDetails;
            existing.CoverageAmount = policy.CoverageAmount;
            existing.CoverageType = policy.CoverageType;
            existing.PremiumAmount = policy.PremiumAmount;
            existing.StartDate = policy.StartDate;
            existing.EndDate = policy.EndDate;
            existing.PolicyStatus = policy.PolicyStatus;

            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await dbContext.Policy.FirstOrDefaultAsync(p => p.PolicyId == id);
            if (existing == null) return false;
            dbContext.Policy.Remove(existing);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
} 