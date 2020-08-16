using Microsoft.AspNetCore.TestHost;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace WebApiExample.Tests
{
    public class BaseFixture
    {
        public TestServer testServer = new TestServer(new WebHostBuilder()
            .UseStartup<Startup>()
            .ConfigureTestServices(collection =>
            {
                collection.AddControllers();
                collection.AddSingleton<LogService, MockLogService>();
            }));

        public T GetService<T>() => this.testServer.Services.GetService<T>();

        public string PopLog() => ((MockLogService)this.GetService<LogService>()).Logs.Dequeue();

        public LogContext<T> PopLog<T>() => JsonSerializer.Deserialize<LogContext<T>>(this.PopLog());

        public HttpClient GetClient() => this.testServer.CreateClient();
    }
}
