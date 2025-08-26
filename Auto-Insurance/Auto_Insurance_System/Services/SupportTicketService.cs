using Auto_Insurance_System.Data;
using Auto_Insurance_System.Models;
using Auto_Insurance_System.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Auto_Insurance_System.Services
{
    public class SupportTicketService : ISupportTicketService
    {
        private readonly AutoInsuranceDbContext context;
        public SupportTicketService(AutoInsuranceDbContext db)
        {
            context = db;
        }
        public bool CreateTicket(SupportTicket ticket) 
        {
            try
            {
                DbSet<SupportTicket> tickets = context.SupportTickects;
                tickets.Add(ticket);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public SupportTicket GetTickectDetails(int id) 
        {
            return context.SupportTickects.Include(t=>t.User).FirstOrDefault(t => t.TicketId == id);
        }

        public bool ResolvedTickect(int ticketId)
        {
            var t = context.SupportTickects.Find(ticketId);
            if (t == null) return false;
            t.TicketStatus = TicketStatus.RESOLVED;
            t.ResolvedDate = DateTime.Now;
            context.SaveChanges();
            return true;
        }

        public bool AssignTicket(int ticketId, int agentUserId)
        {
            var t = context.SupportTickects.Find(ticketId);
            if (t == null) return false;
            t.AssignedAgentId = agentUserId;
            context.SaveChanges();
            return true;
        }

        public List<SupportTicket> GetAllTickects() 
        {
            return context.SupportTickects.Include(t => t.User).ToList();
        }

    }

    
}
