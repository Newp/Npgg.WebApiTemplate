using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApiExample
{
    public class TimeService
    {
        public virtual DateTime GetNow() => DateTime.Now;
    }

    public class LogService
    {
        private readonly ILogger logger;
        private readonly TimeService time;

        public LogService(ILoggerProvider provider, TimeService time)
        {
            this.logger = provider?.CreateLogger("test");
            //this.logger = logger;
            this.time = time;
        }

        public void Write<T>(string subject, T message)
        {
            this.Write(new LogContext<T>()
            {
                Subject = subject,
                Time = time.GetNow(),
                Value = message
            });
        }

        protected virtual void Write(object message)
        {
            var json = JsonSerializer.Serialize(message);

            logger.LogDebug(json);
            //Console.WriteLine(json);
        }
    }

    public class LogContext<T>
    {
        public string Subject { get; set; }
        public DateTime Time { get; set; }
        public T Value { get; set; }
    }
}
