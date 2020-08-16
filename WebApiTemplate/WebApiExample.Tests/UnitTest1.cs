using Microsoft.AspNetCore.TestHost;
using System;
using System.Text.Json;
using Xunit;

using WebApiExample;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebApiExample.Middleware;
using System.Net;

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

    public class BaseFixture
    {
        public TestServer testServer = new TestServer(new WebHostBuilder()
            .UseStartup<Startup>()
            .ConfigureTestServices(collection =>
            {
                collection.AddSingleton<LogService, MockLogService>();
            }));

        public T GetService<T>() => this.testServer.Services.GetService<T>();

        public string PopLog() => ((MockLogService)this.GetService<LogService>()).Logs.Dequeue();

        public LogContext<T> PopLog<T>() => JsonSerializer.Deserialize<LogContext<T>>(this.PopLog());

        public HttpClient GetClient() => this.testServer.CreateClient();
    }

    public class UnitTest1 : BaseFixture
    {
        [Fact]
        public async Task ApiLogTest()
        {
            var client = this.GetClient();

            var path = "/api/values/3939";
            var result = await client.GetAsync(path);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            
            var log = this.PopLog<ApiLog>();

            Assert.Equal(path, log.Value.Request.Path.ToLower());
            Assert.Equal((int)result.StatusCode, log.Value.Response.Status);
        }
    }
}
