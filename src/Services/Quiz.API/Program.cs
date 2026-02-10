using MassTransit;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Quiz.API.Extensions;
using Quiz.API.Grpc;
using Scalar.AspNetCore;
using Serilog;

//serilog configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ApplicationName", "Quiz.API")
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();
try 
{
    Log.Information("Quiz.API is starting");

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddMongoDb(builder.Configuration);
    builder.Services.AddMassTransit(x =>
    {
        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(builder.Configuration["RabbitMqSettings:Host"] ?? "localhost");
        });
    });
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddGrpc();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("QuizMaster API")
                   .WithTheme(ScalarTheme.Moon)
                   .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });
    }

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        var server = app.Services.GetRequiredService<IServer>();
        var addresses = server.Features.Get<IServerAddressesFeature>();

        if (addresses != null)
        {
            foreach (var address in addresses.Addresses)
            {
                Console.WriteLine($"\x1b[32m Scalar UI is running on: {address}/scalar/v1\x1b[0m");
            }
        }
    });

    app.MapGrpcService<QuizGrpcService>();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Quiz.API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush(); //guarantee that all logs are flushed before the application exits
}