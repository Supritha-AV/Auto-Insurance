using AutoInsuranceSystemAPI.Models;

namespace AutoInsuranceSystemAPI.Services
{
    public interface IPolicyService
    {
        Task<List<Policy>> GetAllAsync();
        Task<Policy?> GetByIdAsync(int id);
        Task<Policy> CreateAsync(Policy policy);
        Task<bool> UpdateAsync(int id, Policy policy);
        Task<bool> DeleteAsync(int id);
    }
} 