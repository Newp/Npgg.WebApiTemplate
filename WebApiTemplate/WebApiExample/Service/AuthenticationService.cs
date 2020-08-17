using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApiExample.Service
{
    public class AuthenticationService
    {
        public bool CheckAuthentication(string accessToken)
        {
            var tokenInfo = JsonSerializer.Deserialize<AccessToken>(accessToken);

            return tokenInfo.Name != null;
        }
    }

    public class AccessToken
    {
        public string Name { get; set; }
    }
}
