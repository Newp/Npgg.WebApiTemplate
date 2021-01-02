﻿using Xunit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net;
using WebApiExample.Service;
using System.Net.Http;
using WebApiExample.Controllers;
using Newtonsoft.Json;

namespace WebApiExample.Tests
{
    public class IdempotentTests : BaseFixture
    {
        readonly HttpClient client;

        public IdempotentTests()
        {
            this.client = base.GetClient();

            string token = JsonConvert.SerializeObject(new AccessToken() { Name = "unit_test" });

            client.DefaultRequestHeaders.Add("access_token", token);
            ValuesController.PostCount = 0; //테스트용으로 초기화해줌
        }

        [Fact]
        public async Task NoRequestIdBadRequest()
        {
            var path = "/api/values";
            var result = await client.PostAsync(path, new StringContent(""));

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }


        [Fact]
        public async Task IdempotentOk()
        {
            var path = "/api/values";

            //첫번째 요청
            {
                var content = base.CreateContent("test");

                for (int i = 0; i < 10; i++)
                {
                    var response = await client.PostAsync(path, content);

                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.Equal("1", await response.Content.ReadAsStringAsync());
                }
            }

            //두번째 요청
            {
                var content = base.CreateContent("test");
                var response = await client.PostAsync(path, content);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("2", await response.Content.ReadAsStringAsync());
            }
        }
    }
}
