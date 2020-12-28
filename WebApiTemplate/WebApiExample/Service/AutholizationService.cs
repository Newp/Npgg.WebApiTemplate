using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApiExample.Service
{
    public class AutholizationService
    {
        public bool CheckAutholize(AccessToken accessToken, AutholizeType autholizeType) => accessToken.AutholizeTypes?.Contains(autholizeType) == true;
    }
}
