using Microsoft.AspNetCore.TestHost;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System;
using System.Text;
using Newtonsoft.Json;

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

        public T GetService<T>() where T : notnull => this.testServer.Services.GetRequiredService<T>();

        public string PopLog() => ((MockLogService)this.GetService<LogService>()).Logs.Dequeue();

        public LogContext<T> PopLog<T>() => JsonConvert.DeserializeObject<LogContext<T>>(this.PopLog());

        public HttpClient GetClient() => this.testServer.CreateClient();

        public Encoding encoding = new UTF8Encoding(false);

        public StringContent CreateContent(string body)
        {
            var content = new StringContent(body, encoding, "application/json");
            content.Headers.Add("request-id", Guid.NewGuid().ToString());

            return content;
        }
    }
}
