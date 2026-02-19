using Identity.API.Data;
using Identity.API.Interfaces;
using Identity.API.Models;
using Identity.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Resend;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ApplicationName", "QuizMaster.Identity.API")
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

try
{
    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

    Log.Information("Identity.API is starting up...");

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Host.UseSerilog();

    builder.Services.AddDbContext<AppIdentityDbContext>(options =>
       options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    // Identity Configuration
    builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = true; // Essential for "True" Auth flow
    })
    .AddEntityFrameworkStores<AppIdentityDbContext>()
    .AddDefaultTokenProviders();

    builder.Services.AddOptions();
    // Configure Resend Email Client
    builder.Services.AddHttpClient<IResend, ResendClient>();
    builder.Services.Configure<ResendClientOptions>(options =>
    {
        // Value comes from User Secrets or Env Variables
        options.ApiToken = builder.Configuration["Resend:ApiKey"]!;
    });
    builder.Services.AddTransient<IResend, ResendClient>();
    builder.Services.AddTransient<IEmailSender<ApplicationUser>, ResendEmailSender>();

    builder.Services.AddScoped<ITokenService, TokenService>();

    builder.Services.AddScoped<IIdentityService, IdentityService>();

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<IUserContext, UserContext>();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt => {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("Identity.API is running.");

    app.Run();
}
catch (HostAbortedException) 
{
    throw;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Identity.API host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}