using System;
using Microsoft.Extensions.Logging;

namespace WebApiExample
{
    public partial class Program
    {
        class LoggerProvider : ILoggerProvider
        {
            public ILogger CreateLogger(string categoryName)=> new SystemLogger(categoryName);

            public void Dispose() { }

            class SystemLogger : ILogger
            {
                private readonly string categoryName;
                private readonly LogService logService = new LogService();

                public SystemLogger(string categoryName)
                {
                    this.categoryName = categoryName;
                }

                public IDisposable? BeginScope<TState>(TState state) => null;

                public bool IsEnabled(LogLevel logLevel)
                {
                    return true;
                }

                public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
                {
                    var subject = "system log";
                    var log = new
                    {
                        eventId,
                        categoryName,
                        message = formatter(state, exception)
                    };

                    logService.Write(logLevel, subject, log);
                }
            }
        }
    }
}