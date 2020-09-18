using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebApiExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        class pp : ILoggerProvider
        {
            public ILogger CreateLogger(string categoryName)
            {
                return new logger();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            class logger : ILogger
            {
                public IDisposable BeginScope<TState>(TState state)
                {
                    return null;
                }

                public bool IsEnabled(LogLevel logLevel)
                {
                    return true;
                }

                public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
                {
                    Console.WriteLine(state.ToString().Replace('\n', '\r'));
                    //throw new NotImplementedException();
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureLogging(log =>
                    {
                        log.ClearProviders();
                        log.AddProvider(new pp());
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
