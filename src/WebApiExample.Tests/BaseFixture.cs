using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace WebApiExample.Tests
{
    public partial class BaseFixture : Xunit.IAsyncLifetime
    {
        static readonly Counter sequenceCounter = new Counter();

        public readonly int Sequence;
        public readonly TestServer testServer;
        public BaseFixture()
        {
            this.Sequence = sequenceCounter.Increase();
            this.testServer = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureTestServices(collection =>
                {
                    collection.AddControllers();
                    collection.AddSingleton<LogService, MockLogService>();
                }));
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task InitializeAsync() => Task.CompletedTask;

    }
}
