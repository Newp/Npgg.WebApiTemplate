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
            result = JsonSerializer.Deserialize<AccessToken>(accessToken);

            return result?.Name != null;
        }
    }

    public class AccessToken
    {
        public string? Name { get; set; }
        public AutholizeType[]? AutholizeTypes { get; set; }
    }
}
