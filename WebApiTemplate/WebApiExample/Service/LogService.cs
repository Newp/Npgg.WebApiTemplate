
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace WebApiExample
{
    public class LogService
    {

        public void Trace<T>(string subject, T value) where T : notnull => this.Write(LogLevel.Trace, subject, value);
        public void Debug<T>(string subject, T value) where T : notnull =>  this.Write(LogLevel.Debug, subject, value);
        public void Information<T>(string subject, T value) where T : notnull =>  this.Write(LogLevel.Information, subject, value);
        public void Warning<T>(string subject, T value) where T : notnull =>  this.Write(LogLevel.Warning, subject, value);
        public void Error<T>(string subject, T value) where T : notnull =>  this.Write(LogLevel.Error, subject, value);
        public void Critical<T>(string subject, T value) where T : notnull =>  this.Write(LogLevel.Critical, subject, value);

        public virtual void Write<T>(LogLevel logLevel, string subject, T message) where T : notnull
        {
            var log = MakeLog(logLevel,  subject, message);
            var json = JsonSerializer.Serialize(log);
            Console.WriteLine(json);
        }

        protected LogContext<T> MakeLog<T>(LogLevel logLevel, string subject, T value) where T : notnull
        {
            var time = DateTime.Now;
            var log = new LogContext<T>(
                LogLevel : logLevel,
                Subject : subject,
                Time : time,
                Value : value
            );
            return log;
        }
    }


    public record LogContext<T>(
        LogLevel LogLevel,
        string Subject,
        DateTime Time,
        T Value
    );
}
