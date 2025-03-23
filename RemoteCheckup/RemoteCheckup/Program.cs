using RemoteCheckup.Hubs;
using RemoteCheckup.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(action => {
    action.AddDefaultPolicy(builder => {
        builder.WithOrigins("localhost").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddHostedService<PeriodicPerformanceCheckupService>();
builder.Services.AddHostedService<PeriodicProcessCheckupService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapHub<PerformanceCheckupHub>("/api/hubs/performance");
app.MapHub<ProcessesCheckupHub>("/api/hubs/processes");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");
app.MapFallbackToFile("index.html");

app.Run();
