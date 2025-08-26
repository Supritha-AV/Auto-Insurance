using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auto_Insurance_System.Models
{
    [Table("SupportTickets")]
    public class SupportTicket
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TicketId { get; set; }

        [Required(ErrorMessage = "User Id is required")]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public Users User { get; set; } 

        public int? AssignedAgentId { get; set; }

        [Required(ErrorMessage = "Issue description is required")]
        public string IssueDescription { get; set; }

        [Required(ErrorMessage = "Ticket status is required")]
        public TicketStatus TicketStatus { get; set; }

        [Required(ErrorMessage = "Created date is required")]
        public DateTime CreatedDate { get; set; }

        public DateTime? ResolvedDate { get; set; } 
    }

    public enum TicketStatus
    {
        OPEN,
        RESOLVED
    }
}
