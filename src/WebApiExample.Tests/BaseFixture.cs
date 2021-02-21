﻿using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System;
using System.Text;
using Newtonsoft.Json;

namespace WebApiExample.Tests
{
    public class BaseFixture : IDisposable
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

        void IDisposable.Dispose()
        {
            //throw new NotImplementedException();
        }
        public T GetService<T>() where T : notnull => this.testServer.Services.GetRequiredService<T>();
        public T2 GetService<T, T2>() where T : notnull where T2: T => (T2)this.testServer.Services.GetRequiredService<T>();

        public MockLogService Logs => this.GetService<LogService, MockLogService>();

        public string PopLog() => Logs.Logs.Dequeue();

        public LogContext<T> PopLog<T>() => JsonConvert.DeserializeObject<LogContext<T>>(this.PopLog());

        public HttpClient GetClient() => this.testServer.CreateClient();

        public Encoding encoding = new UTF8Encoding(false);

        public StringContent CreateContent(string body)
        {
            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, encoding, "application/json");
            content.Headers.Add("request-id", Guid.NewGuid().ToString());

            return content;
        }

       
    }
}
