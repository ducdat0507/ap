using RemoteCheckup.Hubs;
using RemoteCheckup.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RemoteCheckup.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseMySQL(connectionString);
});
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", options =>
{
    options.Cookie.Name = "remotecheckup-identity";
    options.Cookie.SameSite = SameSiteMode.None;
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = redirectContext =>
        {
            redirectContext.HttpContext.Response.StatusCode = 401;
            return Task.CompletedTask;
        }
    };        
});

builder.Services.AddSignalR();

builder.Services.AddSingleton<PeriodicPerformanceCheckupService>();
builder.Services.AddHostedService(p => p.GetRequiredService<PeriodicPerformanceCheckupService>());
builder.Services.AddSingleton<PeriodicProcessCheckupService>();
builder.Services.AddHostedService(p => p.GetRequiredService<PeriodicProcessCheckupService>());
builder.Services.AddSingleton<PeriodicDatabaseCheckupService>();
builder.Services.AddHostedService(p => p.GetRequiredService<PeriodicDatabaseCheckupService>());

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();    
    context.Database.Migrate();

    ApplicationDbInitializer.SeedDefaultUser(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseRouting(); 
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<PerformanceCheckupHub>("/api/hubs/performance");
app.MapHub<ProcessesCheckupHub>("/api/hubs/processes");
app.MapHub<DatabasesCheckupHub>("/api/hubs/databases"); 
app.MapControllers();
app.MapFallbackToFile("404.html");

app.Run();