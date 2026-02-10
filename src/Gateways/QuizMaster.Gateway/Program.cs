
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ApplicationName", "QuizMaster.Gateway")
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

try
{
    Log.Information("QuizMaster.Gateway is starting");

    var builder = WebApplication.CreateBuilder(args);

    // Add YARP
    builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    var app = builder.Build();

    app.MapReverseProxy();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "QuizMaster.Gateway terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}