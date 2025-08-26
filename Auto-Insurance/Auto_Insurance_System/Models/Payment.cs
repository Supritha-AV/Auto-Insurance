using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Auto_Insurance_System.Models
{
    [Table("Payments")]
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }

        [Required(ErrorMessage = "PolicyId is required")]
        public int PolicyId { get; set; }

        [ForeignKey(nameof(PolicyId))]
        [ValidateNever]
        public Policy? Policy { get; set; } 

        [Required(ErrorMessage = "Payment amount is required")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PaymentAmount { get; set; }

        [Required(ErrorMessage = "Payment date is required")]
        public DateTime PaymentDate { get; set; }

        [Required(ErrorMessage = "Payment status is required")]
        public PaymentStatus PaymentStatus { get; set; }
    }

    public enum PaymentStatus
    {
        SUCCESS,
        FAILED,
        PENDING
    }
}
