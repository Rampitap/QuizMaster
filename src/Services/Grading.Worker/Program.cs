using EventBus.Messages.Protos;
using Grading.Worker.Consumer;
using Grading.Worker.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ApplicationName", "Grading.Worker")
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

try 
{ 
    Log.Information("Grading.Worker is starting");

    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.AddDbContext<GradingDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddMassTransit(x => 
    {
        x.AddConsumer<SubmissionConsumer>();

        x.UsingRabbitMq((context, cfg) => 
        {
            cfg.Host("localhost", "/", h => {
                h.Username("guest");
                h.Password("guest");
            });

            cfg.ConfigureEndpoints(context);
        });
    });

    builder.Services.AddGrpcClient<QuizGrpc.QuizGrpcClient>(o =>
    {
        o.Address = new Uri("https://localhost:7052"); 
    });

    var host = builder.Build();
    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Grading.Worker terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}