using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoInsuranceSystemAPI.Models
{
    [Table("Policy")]
    public class Policy
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PolicyId { get; set; }

        [Required, MaxLength(50)]
        public string PolicyNumber { get; set; }

        [Required, MaxLength(200)]
        public string VehicleDetails { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal CoverageAmount { get; set; }

        [Required, MaxLength(50)]
        public string CoverageType { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PremiumAmount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public PolicyStatus PolicyStatus { get; set; }
    }

    public enum PolicyStatus
    {
        ACTIVE,
        INACTIVE,
        RENEWED
    }
} 