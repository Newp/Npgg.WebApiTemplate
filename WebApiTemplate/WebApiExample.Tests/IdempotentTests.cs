using Xunit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net;
using WebApiExample.Service;
using System.Text.Json;
using System.Net.Http;

namespace WebApiExample.Tests
{
    public class IdempotentTests : BaseFixture
    {
        readonly HttpClient client;

        public IdempotentTests()
        {
            this.client = base.GetClient();

            string token = JsonSerializer.Serialize(new AccessToken() { Name = "unit_test" });

            client.DefaultRequestHeaders.Add("access_token", token);
        }

        [Fact]
        public async Task RequestIdBadRequest()
        {
            var path = "/api/values";
            var result = await client.PostAsync(path, new StringContent(""));

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
    }
}
