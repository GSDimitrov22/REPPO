using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);
if (builder.Configuration["DatabaseProvider"] == "Sqlite")
{
    var directory = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
    Directory.CreateDirectory(directory);
    var connection = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder
    {
        DataSource = Path.Combine(directory, "reppo.db"),
        ForeignKeys = true
    };
    builder.Services.AddDbContext<TransportContext>(options => options.UseSqlite(connection.ToString()));
}
else
{
    builder.Services.AddDbContext<TransportContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<TransportContext>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Denied";
});
builder.Services.AddControllersWithViews(options =>
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    await SeedData.Initialize(scope.ServiceProvider, app.Environment.IsDevelopment());
}
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();
