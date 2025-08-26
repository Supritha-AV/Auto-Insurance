using Auto_Insurance_System.Models;
namespace Auto_Insurance_System.Interfaces
{
    public interface IClaimService
    {
        bool SubmitClaim(Claim obj);
        Claim GetClaimDetails(string id);
        bool UpdateClaimStatus(Claim obj);
        List<Claim> GetAllClaims();
    }
}
