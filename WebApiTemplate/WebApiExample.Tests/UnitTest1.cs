using System;
using Xunit;

using WebApiExample;
using System.Threading.Tasks;
using WebApiExample.Middleware;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApiExample.Service;
using System.Text.Json;
using System.Net.Http;

namespace WebApiExample.Tests
{

    public class UnitTest1 : BaseFixture
    {
        readonly string token = JsonSerializer.Serialize(new AccessToken() { Name = "unit_test" });
        readonly HttpClient client;
        public UnitTest1()
        {
            client = this.GetClient();
        }

        [Fact]
        public async Task GetOkTest()
        {
            var path = "/api/values";
            var result = await client.GetAsync(path);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }



        [Fact]
        public async Task ApiPathLogTest()
        {
            var path = "/api/values/3939";
            var result = await client.GetAsync(path);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var log = this.PopLog<ApiLog>();

            Assert.Equal(path, log.Value.Request.Path.ToLower());
            Assert.Equal((int)result.StatusCode, log.Value.Response.Status);
        }

        [Fact]
        public async Task QueryStringLogTest()
        {
            var word = "abcdefg";
            var querystring = "?a=1&b=2&b=3&pass=" + word;
            var path = "/api/values/3939" + querystring;
            var result = await client.GetAsync(path);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var log = this.PopLog<ApiLog>();

            var resBody = await result.Content.ReadAsStringAsync();

            Assert.EndsWith(word, resBody);
            Assert.Equal(querystring, log.Value.Request.QueryString);
        }

        [Fact]
        public async Task UnauthenticatedTest()
        {
            var result = await client.GetAsync("/api/values/authenticated");

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        

        [Fact]
        public async Task AuthenticatedTest()
        {
            client.DefaultRequestHeaders.Add("access_token", token);

            var result = await client.GetAsync("/api/values/authenticated");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }


        [Fact]
        public async Task AutholizedGetTest()
        {
            string token = JsonSerializer.Serialize(new AccessToken() { Name = "unit_test", AutholizeTypes = new AutholizeType[] { AutholizeType.Subscriber } });
            client.DefaultRequestHeaders.Add("access_token", token); //

            var result = await client.GetAsync("/api/values/autholized");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task UnautholizedGetTest()
        {
            string token = JsonSerializer.Serialize(new AccessToken() { Name = "unit_test", AutholizeTypes = null });
            client.DefaultRequestHeaders.Add("access_token", token); //

            var result = await client.GetAsync("/api/values/autholized");

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }
    }
}
