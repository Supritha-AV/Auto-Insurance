using Auto_Insurance_System.Data;
using Auto_Insurance_System.Interfaces;
using Auto_Insurance_System.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Auto_Insurance_System.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AutoInsuranceDbContext context;

        public PaymentService(AutoInsuranceDbContext db)
        {
            context = db;
        }

        public bool MakePayment(Payment obj)
        {
            try
            {
                var policy = context.Policy.Find(obj.PolicyId);
                if (policy == null) return false;
                if (obj.PaymentAmount != policy.PremiumAmount) return false;

                DbSet<Payment> payments = context.Payments;
                payments.Add(obj);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public Payment GetPaymentDetails(string id)
        {
            try
            {
                int payId = Convert.ToInt32(id);
                DbSet<Payment> payments = context.Payments;
                Payment p = payments.Find(payId);
                return p;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error fetching payment details: " + ex.Message);
                return null;
            }
        }

        public List<Payment> GetPaymentsByPolicy(int policyId)
        {
            try
            {
                DbSet<Payment> payments = context.Payments;
                return payments
                    .Where(p => p.PolicyId == policyId)
                    .Include(p => p.Policy)
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error fetching payments by policy: " + ex.Message);
                return new List<Payment>();
            }
        }
    }
}
