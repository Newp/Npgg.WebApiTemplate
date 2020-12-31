
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WebApiExample.Tests
{
    public class MockLogService : LogService
    {
        public Queue<string> Logs = new Queue<string>();

        public override void Write<T>(LogLevel logLevel, string subject, T message)
        {
            var log = base.MakeLog(logLevel, subject, message);

            var json = JsonConvert.SerializeObject(log);
            Logs.Enqueue(json);
            //System.Diagnostics.Trace.WriteLine(json);
            base.Write(logLevel, subject, message);
        }
    }
}
