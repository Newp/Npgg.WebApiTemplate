using Newtonsoft.Json;

namespace WebApiExample.Service
{
    public class AuthenticationService
    {
        public bool CheckAuthentication(string accessToken, out AccessToken result)
        {
            result = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessToken>(accessToken);

            return result?.Name != null;
        }
    }

    public class AccessToken
    {
        [JsonRequired]
        public string Name { get; set; } = string.Empty;
        public AutholizeType[]? AutholizeTypes { get; set; }
    }
}
