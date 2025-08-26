using Auto_Insurance_System.Data;
using Auto_Insurance_System.Interfaces;
using Auto_Insurance_System.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Auto_Insurance_System.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly AutoInsuranceDbContext context;
        public PolicyService(AutoInsuranceDbContext db)
        {
            context = db;
        }

        public bool CreatePolicy(Policy obj)
        {
            try
            {
                DbSet<Policy> policies = context.Policy;
                policies.Add(obj);
                context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

        }

        public List<Policy> GetAllPolicies()
        {
            DbSet<Policy> policies = context.Policy;
            return policies.ToList();
        }

        public Policy GetPolicyById(string id)
        {
            int policyId = Convert.ToInt32(id);
            DbSet<Policy> policies = context.Policy;
            Policy policy = policies.Find(policyId);
            return policy;
        }

        public bool UpdatePolicy(Policy obj)
        {
            try
            {
                DbSet<Policy> policies = context.Policy;
                Policy policy =  policies.Find(obj.PolicyId);
                if (policy != null)
                {
                    policy.PolicyNumber = obj.PolicyNumber;
                    policy.PolicyStatus = obj.PolicyStatus;
                    policy.VehicleDetails = obj.VehicleDetails;
                    policy.CoverageAmount = obj.CoverageAmount;
                    policy.CoverageType = obj.CoverageType;
                    policy.PremiumAmount = obj.PremiumAmount;
                    policy.StartDate = obj.StartDate;
                    policy.EndDate = obj.EndDate;

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
                Debug.WriteLine("-----------------------------------------------");
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public bool DeletePolicy(string id)
        {
            try
            {
                DbSet<Policy> policies = context.Policy;
                int policyId = Convert.ToInt32(id);
                Policy policy = policies.Find(policyId);
                if (policy != null)
                {
                    policies.Remove(policy);
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
                Debug.WriteLine("-----------------------------------------------");
                Debug.WriteLine(ex.Message);
                return false;
            }
        }   

    }
}
