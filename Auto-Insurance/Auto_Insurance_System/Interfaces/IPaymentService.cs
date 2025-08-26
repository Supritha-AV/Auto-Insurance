using Auto_Insurance_System.Models;

namespace Auto_Insurance_System.Interfaces
{
    public interface IPaymentService
    {
        bool MakePayment(Payment obj);
        Payment GetPaymentDetails(string id);
        List<Payment> GetPaymentsByPolicy(int policyId);
    }
}
