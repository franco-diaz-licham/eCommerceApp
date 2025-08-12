namespace Backend.Configuration;

public static class LoggingServices
{
    /// <summary>
    /// Configures and adds serilog as a service.
    /// </summary>
    public static void AddSerilog(this WebApplicationBuilder builder)
    {
        // Set up logging
        var env = builder.Environment;
        var logPath = builder.Configuration.GetValue<string>("LogFilePath") ?? "Logs\\LogFile.txt";
        var logFile = Path.Combine(env.ContentRootPath, logPath);
        Directory.CreateDirectory(Path.GetDirectoryName(logFile)!);
        Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                                              .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                                              .Enrich.FromLogContext()
                                              .WriteTo.File(logFile, rollingInterval: RollingInterval.Day)
                                              .CreateLogger();

        // force serilog to export error file if problems when startup.
        Serilog.Debugging.SelfLog.Enable(msg =>
        {
            File.AppendAllText(Path.Combine(env.ContentRootPath, "_info", "serilog-selflog.txt"), msg);
        });

        // Plug Serilog into .NET logging as app initialises.
        builder.Host.UseSerilog();
    }
}
