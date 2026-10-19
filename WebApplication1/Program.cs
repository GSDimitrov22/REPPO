using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure Entity Framework and Database Context
builder.Services.AddDbContext<TransportContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") 
                         ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

// 2. Configure Identity and Roles
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<TransportContext>();

// 3. Add Controller and View services
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// 4. Authentication MUST come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// 5. Map Razor Pages (Required for Identity Login/Register UI)
app.MapRazorPages(); 

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();