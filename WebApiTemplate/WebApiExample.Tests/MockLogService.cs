using System.Text.Json;
using System.Collections.Generic;

namespace WebApiExample.Tests
{
    public class MockLogService : LogService
    {
        public Queue<string> Logs = new Queue<string>();

        public MockLogService(TimeService time) : base(time)
        {
        }

        protected override void Write(object message)
        {
            var json = JsonSerializer.Serialize(message);
            Logs.Enqueue(json);
            System.Diagnostics.Trace.WriteLine(json);
        }
    }
}
