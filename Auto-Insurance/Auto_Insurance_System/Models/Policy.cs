using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auto_Insurance_System.Models
{
    [Table("Policy")]
    public class Policy
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PolicyId { get; set; }

        [Required(ErrorMessage = "Policy Number is mandatory"), MaxLength(50)]
        public string PolicyNumber { get; set; }

        [Required(ErrorMessage = "Vehicle Details are mandatory"), MaxLength(200)]
        public string VehicleDetails { get; set; }

        [Required(ErrorMessage = "Coverage Amount is required")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal CoverageAmount { get; set; }

        [Required(ErrorMessage = "Coverage Type is required"), MaxLength(50)]
        public string CoverageType { get; set; }

        [Required(ErrorMessage = "Premium Amount is required")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PremiumAmount { get; set; }

        [Required(ErrorMessage = "Policy Start Date is mandatory")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Policy End Date is mandatory")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Policy Status is required")]
        public PolicyStatus PolicyStatus { get; set; }
    }

    public enum PolicyStatus
    {
        ACTIVE,
        INACTIVE,
        RENEWED
    }
}
