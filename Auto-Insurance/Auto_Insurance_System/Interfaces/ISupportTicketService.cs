using Auto_Insurance_System.Models;

namespace Auto_Insurance_System.Interfaces
{
    public interface ISupportTicketService
    {
        bool CreateTicket(SupportTicket ticket);
        SupportTicket GetTickectDetails(int id);
        bool ResolvedTickect(int ticketId);
        bool AssignTicket(int ticketId, int agentUserId);
        List<SupportTicket> GetAllTickects();
    }
}
