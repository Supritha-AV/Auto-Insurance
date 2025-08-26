using Auto_Insurance_System.Models;

namespace Auto_Insurance_System.Interfaces
{
    public interface IPolicyService
    {
        bool CreatePolicy(Policy obj);
        List<Policy> GetAllPolicies();
        Policy GetPolicyById(string id);
        bool UpdatePolicy(Policy obj);
        bool DeletePolicy(string id);
    }
}
