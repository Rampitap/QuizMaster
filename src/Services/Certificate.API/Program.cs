using Amazon.S3;
using Certificate.API.Consumers;
using Certificate.API.Data;
using Certificate.API.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ApplicationName", "Certificate.API")
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

try
{
    Log.Information("Certificate.API is starting");

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddDbContext<CertificateDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddSingleton<IAmazonS3>(sp =>
    {
        var config = new AmazonS3Config
        {
            ServiceURL = "http://localhost:9000",
            ForcePathStyle = true,
            UseHttp = true
        };
        return new AmazonS3Client("admin", "minio_password", config);
    });
    builder.Services.AddMassTransit(x =>
    {
        x.AddConsumer<QuizPassedConsumer>();

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host("localhost", "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });

            cfg.ConfigureEndpoints(context);
        });
    });
    builder.Services.AddScoped<CertificateGenerator>();
    builder.Services.AddScoped<S3StorageService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Certificate.API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}