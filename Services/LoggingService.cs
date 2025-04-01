using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace NeuroApp.Services
{
    public class LoggingService
    {
        private static ILogger<LoggingService> _logger;

        public static void Initialize()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Debug()
                .WriteTo.File("logs/neuro-app-.txt", 
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(dispose: true);
            });

            _logger = loggerFactory.CreateLogger<LoggingService>();
        }

        public static void LogInformation(string message)
        {
            _logger?.LogInformation(message);
        }

        public static void LogWarning(string message)
        {
            _logger?.LogWarning(message);
        }

        public static void LogError(string message, Exception ex = null)
        {
            if (ex != null)
            {
                _logger?.LogError(ex, message);
            }
            else
            {
                _logger?.LogError(message);
            }
        }

        public static void LogDebug(string message)
        {
            _logger?.LogDebug(message);
        }
    }
} 