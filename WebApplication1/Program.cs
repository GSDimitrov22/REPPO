using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApplication1.Data;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

// Запис на грешките във файл
var logPath = Path.Combine(
    builder.Environment.ContentRootPath,
    "App_Data", "Logs", "errors-.log");

builder.Logging.AddSerilog(
    new LoggerConfiguration()
        .MinimumLevel.Error()
        .WriteTo.File(
            logPath,
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 14,
            shared: true)
        .CreateLogger(),
    dispose: true);

// Връзка с базата според избрания профил в Rider
if (builder.Configuration["DatabaseProvider"] == "Sqlite")
{
    var directory = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
    Directory.CreateDirectory(directory);

    var connection = new SqliteConnectionStringBuilder
    {
        DataSource = Path.Combine(directory, "reppo.db"),
        ForeignKeys = true
    };

    builder.Services.AddDbContext<TransportContext>(options =>
        options.UseSqlite(connection.ToString()));
}
else
{
    builder.Services.AddDbContext<TransportContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Потребители и роли
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<TransportContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Denied";
});

// Административни справки
builder.Services.AddScoped<IReportSource, DatabaseReportSource>();
builder.Services.AddScoped<ReportService>();

builder.Services.AddControllersWithViews(options =>
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

var app = builder.Build();

// Създаване на базата и началните данни
using (var scope = app.Services.CreateScope())
{
    await SeedData.Initialize(
        scope.ServiceProvider,
        app.Environment.IsDevelopment());
}

// Обработка на грешки при HTTP заявки
app.UseExceptionHandler("/errors/500");
app.UseStatusCodePagesWithReExecute("/errors/{0}");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();