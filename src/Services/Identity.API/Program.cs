using Identity.API.Data;
using Identity.API.Interfaces;
using Identity.API.Models;
using Identity.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Resend;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ApplicationName", "QuizMaster.Identity.API")
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

try
{
    Log.Information("Identity.API is starting up...");

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Host.UseSerilog();
    builder.Services.AddDbContext<AppIdentityDbContext>(options =>
       options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = true; // Essential for "True" Auth flow
    })
    .AddEntityFrameworkStores<AppIdentityDbContext>()
    .AddDefaultTokenProviders();
    builder.Services.AddOptions();
    builder.Services.AddHttpClient<IResend, ResendClient>();
    builder.Services.Configure<ResendClientOptions>(options =>
    {
        // Value comes from User Secrets or Env Variables
        options.ApiKey = builder.Configuration["Resend:ApiKey"]!;
    });
    builder.Services.AddTransient<IResend, ResendClient>();
    builder.Services.AddTransient<IEmailSender<ApplicationUser>, ResendEmailSender>();
    builder.Services.AddScoped<TokenService>();
    builder.Services.AddScoped<IIdentityService, IdentityService>();


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    Log.Information("Identity.API is running.");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Identity.API host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}