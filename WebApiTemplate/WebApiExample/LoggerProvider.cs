using System;
using Microsoft.Extensions.Logging;

namespace WebApiExample
{
    public partial class Program
    {
        class LoggerProvider : ILoggerProvider
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
                public IDisposable? BeginScope<TState>(TState state) => null;

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
    }
}
