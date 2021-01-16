using System;
using System.Linq;

namespace WebApiExample.Service
{
    public class AutholizationService
    {
        public bool CheckAutholize(AccessToken accessToken, AutholizeType autholizeType) => accessToken.AutholizeTypes?.Contains(autholizeType) == true;
    }
}
