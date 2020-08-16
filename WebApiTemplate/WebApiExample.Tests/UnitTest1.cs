using System;
using Xunit;

using WebApiExample;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using WebApiExample.Middleware;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace WebApiExample.Tests
{

    public class UnitTest1 : BaseFixture
    {
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

            var log = this.PopLog<ApiLog>();

            var resBody = await result.Content.ReadAsStringAsync();

            Assert.EndsWith(word, resBody);
            Assert.Equal(querystring, log.Value.Request.QueryString);
        }
    }
}
