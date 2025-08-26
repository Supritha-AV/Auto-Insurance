using Auto_Insurance_System.Data;
using Auto_Insurance_System.Interfaces;
using Auto_Insurance_System.Services;
using Microsoft.EntityFrameworkCore;
using Auto_Insurance_System.Filters;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContextPool<AutoInsuranceDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon")));
builder.Services.AddTransient<IPolicyService, PolicyService>();
builder.Services.AddTransient<IClaimService, ClaimService>();
builder.Services.AddTransient<IPaymentService, PaymentService>();
builder.Services.AddTransient<ISupportTicketService, SupportTicketService>();
builder.Services.AddTransient<IUsersService, UsersService>();
builder.Services.AddSession();
// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
	options.Filters.Add<AutoInsuranceExceptionFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Landing}/{id?}")
	.WithStaticAssets();


app.Run();
