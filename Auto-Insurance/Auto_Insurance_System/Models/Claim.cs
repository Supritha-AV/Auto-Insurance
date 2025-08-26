using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Auto_Insurance_System.Models
{
    [Table("Claims")]
    public class Claim
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClaimId { get; set; }

        //[ForeignKey("Policy")]
        [Required(ErrorMessage = "PolicyId is required")]
        public int PolicyId { get; set; }

        [ValidateNever]
        public Policy? Policy { get; set; }  // Navigation property (optional)

        [Required(ErrorMessage = "Claim amount is required")]
        //[Column(TypeName = "decimal(10,2)")]
        public decimal ClaimAmount { get; set; }

        [Required(ErrorMessage = "Claim date is mandatory")]
        public DateTime ClaimDate { get; set; }

        [Required(ErrorMessage = "Claim status is required")]
        public ClaimStatus ClaimStatus { get; set; }

        [ForeignKey("Adjuster")]
        [Required(ErrorMessage = "AdjusterId is required")]
        public int AdjusterId { get; set; }

        [ValidateNever]
        public Users? Adjuster { get; set; }
    }

    public enum ClaimStatus
    {
        OPEN,
        APPROVED,
        REJECTED
    }
}
