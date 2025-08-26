using Auto_Insurance_System.Data;
using Auto_Insurance_System.Interfaces;
using Auto_Insurance_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Auto_Insurance_System.Filters;

namespace Auto_Insurance_System.Controllers
{
	[RoleAuthorizationFilter("AGENT")]
	public class AgentController : Controller
	{
		private readonly IUsersService usersService;
		private readonly IPolicyService policyService;
		private readonly IClaimService claimService;
		private readonly ISupportTicketService supportTicketService;
		private readonly AutoInsuranceDbContext dbContext;

		public AgentController(
			IUsersService usersService,
			IPolicyService policyService,
			IClaimService claimService,
			ISupportTicketService supportTicketService,
			AutoInsuranceDbContext dbContext)
		{
			this.usersService = usersService;
			this.policyService = policyService;
			this.claimService = claimService;
			this.supportTicketService = supportTicketService;
			this.dbContext = dbContext;
		}

		private bool IsAgent()
		{
			return HttpContext.Session.GetString("UserRole") == "AGENT";
		}

		// Users
		public IActionResult UsersHub()
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			return View();
		}

		public IActionResult Users()
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			var users = usersService.GetAllUsers();
			return View(users);
		}

		[HttpGet]
		public IActionResult UserCreate()
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			return View(new Users { Role = UserRole.CUSTOMER });
		}

		[HttpPost]
		public IActionResult UserCreate(Users model)
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			if (!ModelState.IsValid)
			{
				ViewBag.Error = "Please correct the errors and try again.";
				return View(model);
			}
			if (model.Role == UserRole.ADMIN)
			{
				ViewBag.Error = "Agents cannot create ADMIN users.";
				return View(model);
			}
			var ok = usersService.RegisterUser(model);
			if (ok)
			{
				TempData["Success"] = "User created successfully.";
				return RedirectToAction("UserCreate");
			}
			ViewBag.Error = "Failed to create user.";
			return View(model);
		}

		// Policies
		public IActionResult PoliciesHub()
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			return View();
		}

		public IActionResult PoliciesAll()
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			var list = policyService.GetAllPolicies().OrderBy(p => p.PolicyId).ToList();
			return View(list);
		}

		[HttpGet]
		public IActionResult PolicyDetails()
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			return View(model: null);
		}

		[HttpPost]
		public IActionResult PolicyDetails(int policyId)
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			var policy = policyService.GetPolicyById(policyId.ToString());
			if (policy == null) ViewBag.Error = $"No policy found with id {policyId}";
			return View(policy);
		}

		[HttpGet]
		public IActionResult PolicyCreate()
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			return View(new Policy {
                PolicyNumber = (new Random()).Next(10000, 99999).ToString(),
                StartDate = DateTime.Today, EndDate = DateTime.Today.AddYears(1), PolicyStatus = PolicyStatus.ACTIVE });
		}

		[HttpPost]
		public IActionResult PolicyCreate(Policy model)
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			if (!ModelState.IsValid)
			{
				ViewBag.Error = "Please correct the errors and try again.";
				return View(model);
			}
			var ok = policyService.CreatePolicy(model);
			if (ok)
			{
				TempData["Success"] = "Policy created successfully.";
				return RedirectToAction("PolicyCreate");
			}
			ViewBag.Error = "Failed to create policy.";
			return View(model);
		}

		// Claims
		public IActionResult ClaimsHub()
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			return View();
		}

		public IActionResult ClaimsAll()
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			var claims = claimService.GetAllClaims().OrderBy(c => c.ClaimDate).ToList();
			return View(claims);
		}

		[HttpGet]
		public IActionResult ClaimDetails()
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			return View(model: null);
		}

		[HttpPost]
		public IActionResult ClaimDetails(int claimId)
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			var claim = claimService.GetClaimDetails(claimId.ToString());
			if (claim == null)
			{
				ViewBag.Error = $"No claim found with id {claimId}";
			}
			return View(claim);
		}

		[HttpGet]
		public IActionResult ClaimSubmit()
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			return View();
		}

		[HttpPost]
		public IActionResult ClaimSubmit(Claim model)
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			if (!ModelState.IsValid)
			{
				ViewBag.Error = "Please fill all required fields.";
				return View(model);
			}
			var ok = claimService.SubmitClaim(model);
			if (ok)
			{
				TempData["Success"] = "Claim submitted successfully.";
				return RedirectToAction("ClaimSubmit");
			}
			ViewBag.Error = "Failed to submit claim.";
			return View(model);
		}

		// Payments
		public IActionResult PaymentsHub()
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			return View();
		}

		public IActionResult PaymentsAll()
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			var payments = dbContext.Payments
				.AsNoTracking()
				.Include(p => p.Policy)
				.OrderBy(p => p.PaymentDate)
				.ToList();
			return View("Payments", payments);
		}

		[HttpGet]
		public IActionResult PaymentsByPolicy()
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			return View(model: null);
		}

		[HttpPost]
		public IActionResult PaymentsByPolicy(int policyId)
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			var list = dbContext.Payments.Include(p => p.Policy).Where(p => p.PolicyId == policyId).OrderBy(p => p.PaymentDate).ToList();
			if (!list.Any()) ViewBag.Error = $"No payments found for policy {policyId}";
			return View(list);
		}

		[HttpGet]
		public IActionResult PaymentMake()
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			return View(new Payment { PaymentDate = DateTime.Today, PaymentStatus = PaymentStatus.SUCCESS });
		}

		[HttpPost]
		public IActionResult PaymentMake(Payment model)
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			if (!ModelState.IsValid)
			{
				ViewBag.Error = "Please fill all required fields.";
				return View(model);
			}
			// Always record agent payments as SUCCESS
			model.PaymentStatus = PaymentStatus.SUCCESS;
			var policy = dbContext.Policy.Find(model.PolicyId);
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
			var paymentService = new Services.PaymentService(dbContext);
			var ok = paymentService.MakePayment(model);
			if (ok)
			{
				TempData["Success"] = "Payment recorded successfully.";
				return RedirectToAction("PaymentMake");
			}
			ViewBag.Error = "Failed to record payment.";
			return View(model);
		}

		// Tickets
		public IActionResult TicketsHub()
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			return View();
		}

		public IActionResult Tickets()
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			var tickets = supportTicketService.GetAllTickects();
			return View(tickets);
		}

		[HttpGet]
		public IActionResult TicketCreate()
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			return View(new SupportTicket{ CreatedDate = DateTime.Now, TicketStatus = TicketStatus.OPEN });
		}

		[HttpPost]
		public IActionResult TicketCreate(SupportTicket model)
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			// Normalize server-side fields and avoid validating navigation property
			ModelState.Remove(nameof(SupportTicket.CreatedDate));
			ModelState.Remove(nameof(SupportTicket.User));
			model.CreatedDate = DateTime.Now;
			if (model.TicketStatus == 0) model.TicketStatus = TicketStatus.OPEN;

			if (!ModelState.IsValid)
			{
				ViewBag.Error = "Please fill all required fields.";
				return View(model);
			}

			// Validate FK: User must exist
			if (!dbContext.Users.Any(u => u.UserId == model.UserId))
			{
				ViewBag.Error = $"User with id {model.UserId} not found.";
				return View(model);
			}

			var ok = supportTicketService.CreateTicket(model);
			if (ok)
			{
				TempData["Success"] = "Ticket created successfully.";
				return RedirectToAction("TicketCreate");
			}
			ViewBag.Error = "Failed to create ticket.";
			return View(model);
		}

		[HttpGet]
		public IActionResult TicketResolve()
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			return View(model: null);
		}

		[HttpPost]
		public IActionResult TicketResolve(int ticketId)
		{
			if (!IsAgent()) return RedirectToAction("Login", "Auth");
			var ticket = dbContext.SupportTickects.Find(ticketId);
			if (ticket == null)
			{
				ViewBag.Error = $"No ticket found with id {ticketId}";
				return View();
			}
			var currentUser = HttpContext.Session.GetString("UserName");
			var agent = usersService.GetAllUsers().FirstOrDefault(u => u.Username == currentUser);
			if (ticket.AssignedAgentId != agent?.UserId)
			{
				ViewBag.Error = "Ticket is not assigned to you. You cannot resolve it.";
				return View();
			}
			var ok = supportTicketService.ResolvedTickect(ticketId);
			if (!ok)
			{
				ViewBag.Error = "Failed to resolve ticket.";
				return View();
			}
			ViewBag.Success = "Ticket resolved.";
			return View();
		}
	}
} 