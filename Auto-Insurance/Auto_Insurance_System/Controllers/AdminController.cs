using Auto_Insurance_System.Data;
using Auto_Insurance_System.Interfaces;
using Auto_Insurance_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Auto_Insurance_System.Filters;

namespace Auto_Insurance_System.Controllers
{
	[RoleAuthorizationFilter("ADMIN")]
	public class AdminController : Controller
	{
		private readonly IUsersService usersService;
		private readonly IPolicyService policyService;
		private readonly IClaimService claimService;
		private readonly ISupportTicketService supportTicketService;
		private readonly AutoInsuranceDbContext dbContext;

		public AdminController(
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

		private bool IsAdmin()
		{
			return HttpContext.Session.GetString("UserRole") == "ADMIN";
		}

		private void PopulateDashboardStats()
		{
			var policies = policyService.GetAllPolicies();
			var claims = claimService.GetAllClaims();
			var tickets = supportTicketService.GetAllTickects();
			var users = usersService.GetAllUsers();
			var payments = dbContext.Payments.AsNoTracking().ToList();

			ViewBag.TotalPolicies = policies.Count;
			ViewBag.ActivePolicies = policies.Count(p => p.PolicyStatus == PolicyStatus.ACTIVE);
			ViewBag.PendingClaims = claims.Count(c => c.ClaimStatus == ClaimStatus.OPEN);
			ViewBag.ApprovedClaims = claims.Count(c => c.ClaimStatus == ClaimStatus.APPROVED);
			ViewBag.OpenTickets = tickets.Count(t => t.TicketStatus == TicketStatus.OPEN);
			ViewBag.TotalUsers = users.Count;

			var now = DateTime.Now;
			ViewBag.RevenueThisMonth = payments
				.Where(p => p.PaymentStatus == PaymentStatus.SUCCESS && p.PaymentDate.Month == now.Month && p.PaymentDate.Year == now.Year)
				.Sum(p => (decimal?)p.PaymentAmount) ?? 0m;
		}

		public IActionResult Index()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			return View();
		}

		public IActionResult Console()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			return View();
		}

		public IActionResult UsersHub()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			var users = usersService.GetAllUsers();
			return View(users);
		}

		public IActionResult Users()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			var users = usersService.GetAllUsers();
			return View(users);
		}

		[HttpGet]
		public IActionResult UserCreate()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			return View(new Users());
		}

		[HttpPost]
		public IActionResult UserCreate(Users model)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var created = usersService.RegisterUser(model);
			if (!created)
			{
				ViewBag.Error = "Failed to create user.";
				return View(model);
			}
			TempData["Success"] = "User created successfully.";
			return RedirectToAction("UserCreate");
		}

		[HttpGet]
		public IActionResult UserUpdate()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			return View(model: null);
		}

		[HttpPost]
		public IActionResult UserUpdate(Users model)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			if (model.UserId <= 0)
			{
				ViewBag.Error = "UserId is required for update.";
				return View(model);
			}
			var existing = dbContext.Users.Find(model.UserId);
			if (existing == null)
			{
				ViewBag.Error = $"No user found with id {model.UserId}";
				return View(model);
			}
			existing.Email = model.Email;
			existing.Username = model.Username;
			existing.Role = model.Role;
			if (!string.IsNullOrWhiteSpace(model.Password)) existing.Password = model.Password;
			dbContext.SaveChanges();
			TempData["Success"] = "User updated successfully.";
			return RedirectToAction("UserUpdate");
		}

		[HttpGet]
		public IActionResult UserDelete()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			return View();
		}

		[HttpPost]
		public IActionResult UserDelete(int userId)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			var existing = dbContext.Users.Find(userId);
			if (existing == null)
			{
				ViewBag.Error = $"No user found with id {userId}";
				return View();
			}
			dbContext.Users.Remove(existing);
			dbContext.SaveChanges();
			TempData["Success"] = "User deleted successfully.";
			return RedirectToAction("UserDelete");
		}

		// Policies hub + pages
		public IActionResult Policies()
		{
			return RedirectToAction("PoliciesHub");
		}

		public IActionResult PoliciesHub()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			return View();
		}

		public IActionResult PoliciesAll()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			var list = policyService.GetAllPolicies().OrderBy(p => p.PolicyId).ToList();
			return View(list);
		}

		[HttpGet]
		public IActionResult PolicyCreate()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			return NotFound();
		}

		[HttpPost]
		public IActionResult PolicyCreate(Policy model)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			return NotFound();
		}

		[HttpGet]
		public IActionResult PolicyDetails()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			return View(model: null);
		}

		[HttpPost]
		public IActionResult PolicyDetails(int policyId)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			var policy = policyService.GetPolicyById(policyId.ToString());
			if (policy == null) ViewBag.Error = $"No policy found with id {policyId}";
			return View(policy);
		}

		[HttpGet]
		public IActionResult PolicyUpdate()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			return View(model: null);
		}

		[HttpPost]
		public IActionResult PolicyUpdate(Policy model)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			if (model.PolicyId <= 0)
			{
				ViewBag.Error = "PolicyId is required for update.";
				return View(model);
			}
			var ok = policyService.UpdatePolicy(model);
			if (ok)
			{
				TempData["Success"] = "Policy updated successfully.";
				return RedirectToAction("PolicyUpdate");
			}
			ViewBag.Error = "Failed to update policy.";
			return View(model);
		}

		[HttpGet]
		public IActionResult PolicyDelete()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			return View();
		}

		[HttpPost]
		public IActionResult PolicyDelete(int policyId)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			var ok = policyService.DeletePolicy(policyId.ToString());
			if (ok)
			{
				TempData["Success"] = "Policy deleted successfully.";
				return RedirectToAction("PolicyDelete");
			}
			ViewBag.Error = "Failed to delete policy.";
			return View();
		}

		// Claims hub and pages
		public IActionResult Claims()
		{
			return RedirectToAction("ClaimsHub");
		}

		public IActionResult ClaimsHub()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			return View();
		}

		public IActionResult ClaimsAll()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			var claims = claimService.GetAllClaims().OrderBy(c => c.ClaimDate).ToList();
			return View(claims);
		}

		[HttpGet]
		public IActionResult ClaimDetails()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			return View(model: null);
		}

		[HttpPost]
		public IActionResult ClaimDetails(int claimId)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
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
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			return View();
		}

		[HttpPost]
		public IActionResult ClaimSubmit(Claim model)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
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

		[HttpGet]
		public IActionResult ClaimUpdateStatus()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			return View();
		}

		[HttpPost]
		public IActionResult ClaimUpdateStatus(int claimId, ClaimStatus status, int? adjusterId)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			var claim = dbContext.Claims.Find(claimId);
			if (claim == null)
			{
				ViewBag.Error = $"No claim found with id {claimId}";
				return View();
			}
			claim.ClaimStatus = status;
			if (adjusterId.HasValue) claim.AdjusterId = adjusterId.Value;
			dbContext.SaveChanges();
			ViewBag.Success = "Claim status updated.";
			return View();
		}

		// Payments hub and pages
		public IActionResult Payments()
		{
			return RedirectToAction("PaymentsHub");
		}

		public IActionResult PaymentsHub()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			return View();
		}

		public IActionResult PaymentsAll()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			var payments = dbContext.Payments
				.AsNoTracking()
				.Include(p => p.Policy)
				.OrderBy(p => p.PaymentDate)
				.ToList();
			return View("Payments", payments);
		}

		[HttpGet]
		public IActionResult PaymentMake()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			return NotFound();
		}

		[HttpPost]
		public IActionResult PaymentMake(Payment model)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			return NotFound();
		}

		[HttpGet]
		public IActionResult PaymentUpdateStatus()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			return View();
		}

		[HttpPost]
		public IActionResult PaymentUpdateStatus(int paymentId, PaymentStatus status)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			var pay = dbContext.Payments.Find(paymentId);
			if (pay == null)
			{
				ViewBag.Error = $"No payment found with id {paymentId}";
				return View();
			}
			pay.PaymentStatus = status;
			dbContext.SaveChanges();
			ViewBag.Success = "Payment status updated.";
			return View();
		}

		[HttpGet]
		public IActionResult PaymentsByPolicy()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			return View(model: null);
		}

		[HttpPost]
		public IActionResult PaymentsByPolicy(int policyId)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			var list = dbContext.Payments.Include(p => p.Policy).Where(p => p.PolicyId == policyId).OrderBy(p => p.PaymentDate).ToList();
			if (!list.Any()) ViewBag.Error = $"No payments found for policy {policyId}";
			return View(list);
		}

		public IActionResult Tickets()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			var tickets = supportTicketService.GetAllTickects();
			return View(tickets);
		}

		// Tickets hub and pages
		public IActionResult TicketsHub()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			return View();
		}

		public IActionResult TicketsAll()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			var tickets = supportTicketService.GetAllTickects();
			return View(tickets);
		}

		[HttpGet]
		public IActionResult TicketAssign()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			return View(model: null);
		}

		[HttpPost]
		public IActionResult TicketAssign(int ticketId, int agentId)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			var ok = supportTicketService.AssignTicket(ticketId, agentId);
			if (!ok)
			{
				ViewBag.Error = $"Unable to assign ticket {ticketId}.";
				return View();
			}
			ViewBag.Success = "Ticket assigned.";
			return View();
		}

		[HttpGet]
		public IActionResult TicketResolve()
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			return View(model: null);
		}

		[HttpPost]
		public IActionResult TicketResolve(int ticketId)
		{
			if (!IsAdmin()) return RedirectToAction("Login", "Auth");
			PopulateDashboardStats();
			var ok = supportTicketService.ResolvedTickect(ticketId);
			if (!ok)
			{
				ViewBag.Error = $"No ticket found with id {ticketId}";
				return View();
			}
			ViewBag.Success = "Ticket resolved.";
			return View();
		}

		// JSON endpoints for admin operations -------------------------
		[HttpPost]
		public IActionResult AdminCreateUser([FromForm] Users user)
		{
			if (!IsAdmin()) return Unauthorized();
			if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password) || string.IsNullOrWhiteSpace(user.Email))
				return BadRequest("Invalid user input");
			var ok = usersService.RegisterUser(user);
			return ok ? Ok(new { message = "User created" }) : BadRequest("Failed to create user");
		}

		[HttpPost]
		public IActionResult AdminUpdateUser([FromForm] Users user)
		{
			if (!IsAdmin()) return Unauthorized();
			var existing = dbContext.Users.Find(user.UserId);
			if (existing == null) return NotFound();
			existing.Email = user.Email;
			existing.Username = user.Username;
			existing.Role = user.Role;
			if (!string.IsNullOrWhiteSpace(user.Password)) existing.Password = user.Password;
			dbContext.SaveChanges();
			return Ok(new { message = "User updated" });
		}

		[HttpPost]
		public IActionResult AdminDeleteUser([FromForm] int userId)
		{
			if (!IsAdmin()) return Unauthorized();
			var existing = dbContext.Users.Find(userId);
			if (existing == null) return NotFound();
			dbContext.Users.Remove(existing);
			dbContext.SaveChanges();
			return Ok(new { message = "User deleted" });
		}

		[HttpPost]
		public IActionResult AdminCreatePolicy([FromForm] Policy model)
		{
			if (!IsAdmin()) return Unauthorized();
			var ok = policyService.CreatePolicy(model);
			return ok ? Ok(new { message = "Policy created" }) : BadRequest("Failed to create policy");
		}

		[HttpPost]
		public IActionResult AdminUpdatePolicy([FromForm] Policy model)
		{
			if (!IsAdmin()) return Unauthorized();
			var ok = policyService.UpdatePolicy(model);
			return ok ? Ok(new { message = "Policy updated" }) : BadRequest("Failed to update policy");
		}

		[HttpPost]
		public IActionResult AdminDeletePolicy([FromForm] int policyId)
		{
			if (!IsAdmin()) return Unauthorized();
			var ok = policyService.DeletePolicy(policyId.ToString());
			return ok ? Ok(new { message = "Policy deleted" }) : BadRequest("Failed to delete policy");
		}

		[HttpGet]
		public IActionResult AdminGetPolicy(int id)
		{
			if (!IsAdmin()) return Unauthorized();
			var policy = policyService.GetPolicyById(id.ToString());
			if (policy == null) return NotFound();
			return Json(policy);
		}

		[HttpPost]
		public IActionResult AdminSubmitClaim([FromForm] Claim model)
		{
			if (!IsAdmin()) return Unauthorized();
			var ok = claimService.SubmitClaim(model);
			return ok ? Ok(new { message = "Claim submitted" }) : BadRequest("Failed to submit claim");
		}

		[HttpPost]
		public IActionResult AdminUpdateClaimStatus([FromForm] int claimId, [FromForm] ClaimStatus status, [FromForm] int? adjusterId)
		{
			if (!IsAdmin()) return Unauthorized();
			var claim = dbContext.Claims.Find(claimId);
			if (claim == null) return NotFound();
			claim.ClaimStatus = status;
			if (adjusterId.HasValue) claim.AdjusterId = adjusterId.Value;
			dbContext.SaveChanges();
			return Ok(new { message = "Claim status updated" });
		}

		[HttpPost]
		public IActionResult AdminResolveTicket([FromForm] int ticketId)
		{
			if (!IsAdmin()) return Unauthorized();
			var ticket = dbContext.SupportTickects.Find(ticketId);
			if (ticket == null) return NotFound();
			ticket.TicketStatus = TicketStatus.RESOLVED;
			ticket.ResolvedDate = DateTime.Now;
			dbContext.SaveChanges();
			return Ok(new { message = "Ticket resolved" });
		}
	}
} 