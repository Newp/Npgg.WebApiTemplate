using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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
