using Auto_Insurance_System.Interfaces;
using Auto_Insurance_System.Models;
using Microsoft.AspNetCore.Mvc;
using Auto_Insurance_System.Data;

namespace Auto_Insurance_System.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUsersService userService;
        private readonly IPolicyService? policyService;
        private readonly IClaimService? claimService;
        private readonly ISupportTicketService? supportTicketService;
        private readonly AutoInsuranceDbContext? dbContext;

        public AuthController(IUsersService userService, IPolicyService? policyService = null, IClaimService? claimService = null, ISupportTicketService? supportTicketService = null, AutoInsuranceDbContext? dbContext = null)
        {
            this.userService = userService;
            this.policyService = policyService;
            this.claimService = claimService;
            this.supportTicketService = supportTicketService;
            this.dbContext = dbContext;
        }

        private void PopulateAdminStats()
        {
            if (policyService == null || claimService == null || supportTicketService == null || dbContext == null)
                return;

            var policies = policyService.GetAllPolicies();
            var claims = claimService.GetAllClaims();
            var tickets = supportTicketService.GetAllTickects();
            var payments = dbContext.Payments.ToList();

            ViewBag.ActivePolicies = policies.Count(p => p.PolicyStatus == PolicyStatus.ACTIVE);
            ViewBag.PendingClaims = claims.Count(c => c.ClaimStatus == ClaimStatus.OPEN);
            var now = DateTime.Now;
            ViewBag.RevenueThisMonth = payments
                .Where(p => p.PaymentStatus == PaymentStatus.SUCCESS && p.PaymentDate.Month == now.Month && p.PaymentDate.Year == now.Year)
                .Sum(p => (decimal?)p.PaymentAmount) ?? 0m;
            ViewBag.OpenTickets = tickets.Count(t => t.TicketStatus == TicketStatus.OPEN);
        }

        // GET: /User/Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Users user)
        {
            try
            {
                user.Role = Enum.Parse<UserRole>(user.Role.ToString(), true);

                if (userService.RegisterUser(user))
                {
                    ViewBag.SuccessMessage = $"Hello {user.Username}, you are successfully registered as {user.Role}.";
                    ViewBag.TriggerRedirect = true;
                    return View();
                }

                ModelState.AddModelError("", "Registration failed. Please try again.");
                return View(user);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error during registration: {ex.Message}");
                return View(user);
            }
        }

        // GET: /Auth/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Auth/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(string? Username, string? Email, string NewPassword, string ConfirmPassword)
        {
            if (dbContext == null)
            {
                ModelState.AddModelError("", "Unexpected error: database not available.");
                return View();
            }

            if (string.IsNullOrWhiteSpace(Username) && string.IsNullOrWhiteSpace(Email))
            {
                ModelState.AddModelError("", "Please provide Username or Email.");
                return View();
            }
            if (string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ModelState.AddModelError("", "Password and Confirm Password are required.");
                return View();
            }
            if (!string.Equals(NewPassword, ConfirmPassword))
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View();
            }

            var user = !string.IsNullOrWhiteSpace(Username)
                ? dbContext.Users.FirstOrDefault(u => u.Username == Username)
                : dbContext.Users.FirstOrDefault(u => u.Email == Email);

            if (user == null)
            {
                ModelState.AddModelError("", "No matching user found.");
                return View();
            }

            user.Password = NewPassword;
            dbContext.SaveChanges();
            TempData["Success"] = "Password updated successfully. Please login.";
            return RedirectToAction("Login");
        }

        // GET: /User/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /User/Login
        [HttpPost]
        public IActionResult Login(string Username, string Password, string Role)
        {
            var userLogin = new Users
            {
                Username = Username,
                Password = Password,
                Role = Enum.Parse<UserRole>(Role)
            };

            if (userService.VerifyUserLogin(userLogin))
            {
                HttpContext.Session.SetString("UserName", Username);
                HttpContext.Session.SetString("UserRole", Role);

                return Role switch
                {
                    "ADMIN" => RedirectToAction("AdminDashboard"),
                    "AGENT" => RedirectToAction("AgentDashboard"),
                    "CUSTOMER" => RedirectToAction("CustomerDashboard"),
                    _ => RedirectToAction("Landing", "Home")
                };
            }

            TempData["LoginError"] = "Invalid username, password, or role.";
            return RedirectToAction("Landing", "Home");
        }


        public IActionResult AdminDashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "ADMIN")
                return RedirectToAction("Login");

            PopulateAdminStats();
            return View();
        }

        public IActionResult AgentDashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "AGENT")
                return RedirectToAction("Login");
            PopulateAdminStats();
            return View();
        }

        public IActionResult CustomerDashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "CUSTOMER")
                return RedirectToAction("Login");
            return View();
        }

        public IActionResult Profile()
        {
            string username = HttpContext.Session.GetString("UserName");
            var user = userService.GetAllUsers().FirstOrDefault(u => u.Username == username);
            return View(user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Landing", "Home");
        }


    }
}
