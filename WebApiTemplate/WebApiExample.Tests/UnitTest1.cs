using System;
using Xunit;

using WebApiExample;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using WebApiExample.Middleware;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApiExample.Service;
using System.Text.Json;

namespace WebApiExample.Tests
{

    public class UnitTest1 : BaseFixture
    {
        [Fact]
        public async Task GetOkTest()
        {
            var client = this.GetClient();

            var path = "/api/values";
            var result = await client.GetAsync(path);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task ApiPathLogTest()
        {
            var client = this.GetClient();

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
            var client = this.GetClient();

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
        public async Task UnautholizedTest()
        {
            var client = this.GetClient();

            var result = await client.GetAsync("/api/values/authenticated");

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task AutholizedTest()
        {
            var token = JsonSerializer.Serialize(new AccessToken() { Name = "unit_test" });
            
            var client = this.GetClient();

            client.DefaultRequestHeaders.Add("access_token", token);

            var result = await client.GetAsync("/api/values/authenticated");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public void ErrorTest()
        {
            Assert.True(false);
        }
    }
}
