using Auto_Insurance_System.Data;
using Auto_Insurance_System.Interfaces;
using Auto_Insurance_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Auto_Insurance_System.Filters;

namespace Auto_Insurance_System.Controllers
{
	[RoleAuthorizationFilter("CUSTOMER")]
	public class CustomerController : Controller
	{
		private readonly IPolicyService policyService;
		private readonly IClaimService claimService;
		private readonly IPaymentService paymentService;
		private readonly ISupportTicketService ticketService;
		private readonly AutoInsuranceDbContext db;

		public CustomerController(IPolicyService policyService, IClaimService claimService, IPaymentService paymentService, ISupportTicketService ticketService, AutoInsuranceDbContext db)
		{
			this.policyService = policyService;
			this.claimService = claimService;
			this.paymentService = paymentService;
			this.ticketService = ticketService;
			this.db = db;
		}

		private bool IsCustomer()
		{
			return HttpContext.Session.GetString("UserRole") == "CUSTOMER";
		}

		private Users? CurrentUser()
		{
			var name = HttpContext.Session.GetString("UserName");
			return db.Users.FirstOrDefault(u => u.Username == name);
		}

		// Dashboard hubs
		public IActionResult PoliciesHub()
		{
			if (!IsCustomer()) return RedirectToAction("Login", "Auth");
			return View();
		}

		public IActionResult ClaimsHub()
		{
			if (!IsCustomer()) return RedirectToAction("Login", "Auth");
			return View();
		}

		public IActionResult PaymentsHub()
		{
			if (!IsCustomer()) return RedirectToAction("Login", "Auth");
			return View();
		}

		public IActionResult TicketsHub()
		{
			if (!IsCustomer()) return RedirectToAction("Login", "Auth");
			return View();
		}

		// Policy: create
		[HttpGet]
		public IActionResult PolicyCreate()
		{
			if (!IsCustomer()) return RedirectToAction("Login", "Auth");
			return View(new Policy {
				PolicyNumber = (new Random()).Next(10000, 99999).ToString(),
				StartDate = DateTime.Today, EndDate = DateTime.Today.AddYears(1), PolicyStatus = PolicyStatus.ACTIVE });
		}

		[HttpPost]
		public IActionResult PolicyCreate(Policy model)
		{
			if (!IsCustomer()) return RedirectToAction("Login", "Auth");
			if (!ModelState.IsValid)
			{
				ViewBag.Error = "Please correct the errors and try again.";
				return View(model);
			}
            //var ok = policyService.CreatePolicy(model);
            //https://localhost:7227/api/admin/policies
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7227/api/");
			var result = client.PostAsJsonAsync("admin/policies", model);
			result.Wait();
			bool ok = false;
			if (result.IsCompleted)
			{
				ok = true;
			}

            if (ok)
			{
				TempData["Success"] = "Policy created successfully.";
				return RedirectToAction("PolicyCreate");
			}
			ViewBag.Error = "Failed to create policy.";
			return View(model);
		}

		// Claims: submit and list my claims
		[HttpGet]
		public IActionResult ClaimSubmit()
		{
			if (!IsCustomer()) return RedirectToAction("Login", "Auth");
			return View(new Claim { ClaimDate = DateTime.Today, ClaimStatus = ClaimStatus.OPEN });
		}

		[HttpPost]
		public IActionResult ClaimSubmit(Claim model)
		{
			if (!IsCustomer()) return RedirectToAction("Login", "Auth");
			if (!ModelState.IsValid)
			{
				ViewBag.Error = "Please fill all required fields.";
				return View(model);
			}
			// Force OPEN status
			model.ClaimStatus = ClaimStatus.OPEN;
			var ok = claimService.SubmitClaim(model);
			if (ok)
			{
				TempData["Success"] = "Claim submitted successfully.";
				return RedirectToAction("ClaimSubmit");
			}
			ViewBag.Error = "Failed to submit claim.";
			return View(model);
		}

		public IActionResult Claims()
		{
			if (!IsCustomer()) return RedirectToAction("Login", "Auth");
			var me = CurrentUser();
			// Assuming AdjusterId is not the customer, use Policy ownership once available; for now show none unless policy filter is used
			var myPolicyIds = db.Policy.Select(p => p.PolicyId).ToList();
			var claims = db.Claims.AsNoTracking()
				.Where(c => myPolicyIds.Contains(c.PolicyId))
				.OrderBy(c => c.ClaimDate)
				.ToList();
			return View(claims);
		}

		// Payments: make and list my payments
		[HttpGet]
		public IActionResult PaymentMake()
		{
			if (!IsCustomer()) return RedirectToAction("Login", "Auth");
			return View(new Payment { PaymentDate = DateTime.Today, PaymentStatus = PaymentStatus.SUCCESS });
		}

		[HttpPost]
		public IActionResult PaymentMake(Payment model)
		{
			if (!IsCustomer()) return RedirectToAction("Login", "Auth");
			if (!ModelState.IsValid)
			{
				ViewBag.Error = "Please fill all required fields.";
				return View(model);
			}
			// Force status to SUCCESS regardless of posted value
			model.PaymentStatus = PaymentStatus.SUCCESS;
			var policy = db.Policy.Find(model.PolicyId);
			if (policy == null)
			{
				ViewBag.Error = $"No policy found with id {model.PolicyId}";
				return View(model);
			}
			if (model.PaymentAmount != policy.PremiumAmount)
			{
				ViewBag.Error = $"Amount must equal the policy premium ({policy.PremiumAmount:0.00}).";
				return View(model);
			}
			var ok = paymentService.MakePayment(model);
			if (ok)
			{
				TempData["Success"] = "Payment recorded successfully.";
				return RedirectToAction("PaymentMake");
			}
			ViewBag.Error = "Failed to record payment.";
			return View(model);
		}

		public IActionResult Payments()
		{
			if (!IsCustomer()) return RedirectToAction("Login", "Auth");
			var me = CurrentUser();
			var myPolicyIds = db.Policy.Select(p => p.PolicyId).ToList();
			var payments = db.Payments.AsNoTracking()
				.Where(p => myPolicyIds.Contains(p.PolicyId))
				.OrderBy(p => p.PaymentDate)
				.ToList();
			return View(payments);
		}

		// Tickets: create and list my tickets
		[HttpGet]
		public IActionResult TicketCreate()
		{
			if (!IsCustomer()) return RedirectToAction("Login", "Auth");
			return View(new SupportTicket { CreatedDate = DateTime.Now, TicketStatus = TicketStatus.OPEN });
		}

		[HttpPost]
		public IActionResult TicketCreate(SupportTicket model)
		{
			if (!IsCustomer()) return RedirectToAction("Login", "Auth");
			ModelState.Remove(nameof(SupportTicket.CreatedDate));
			ModelState.Remove(nameof(SupportTicket.User));
			model.CreatedDate = DateTime.Now;
			model.TicketStatus = TicketStatus.OPEN;
			if (!ModelState.IsValid)
			{
				ViewBag.Error = "Please fill all required fields.";
				return View(model);
			}
			var ok = ticketService.CreateTicket(model);
			if (ok)
			{
				TempData["Success"] = "Ticket created successfully.";
				return RedirectToAction("TicketCreate");
			}
			ViewBag.Error = "Failed to create ticket.";
			return View(model);
		}

		public IActionResult Tickets()
		{
			if (!IsCustomer()) return RedirectToAction("Login", "Auth");
			var me = CurrentUser();
			var tickets = db.SupportTickects.Include(t => t.User).Where(t => t.UserId == me!.UserId).OrderBy(t => t.CreatedDate).ToList();
			return View(tickets);
		}

		public IActionResult Policies()
		{
			if (!IsCustomer()) return RedirectToAction("Login", "Auth");
			var me = CurrentUser();
			var list = db.Policy.AsNoTracking().OrderBy(p => p.PolicyId).ToList();
			return View(list);
		}
	}
} 