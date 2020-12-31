using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgg.Middleware;
using System.Net;
using WebApiExample.Service;
using Microsoft.Extensions.DependencyInjection;

namespace WebApiExample.Middleware
{
    public class AuthenticationMiddleware : MetaDataMiddleware<AnonymousApiAttribute>, IMiddleware
    {
        private readonly AuthenticationInfo authenticationInfo;
        private readonly AuthenticationService authenticationService;

        public AuthenticationMiddleware(AuthenticationInfo authenticationInfo, AuthenticationService authenticationService) : base()
        {
            this.authenticationInfo = authenticationInfo;
            this.authenticationService = authenticationService;
        }

        public override Task Run(HttpContext context, AnonymousApiAttribute? metaData)
        {
            if (metaData != null) return Task.CompletedTask;

            if(context.TryGetHeader("access_token", out var accessToken) == false
                ||  authenticationService.CheckAuthentication(accessToken, out var token) == false)
            {
                throw new HandledException(HttpStatusCode.Unauthorized, "you shall not pass");
            }


            authenticationInfo.SetAuthentication();
            context.SetItem(token);
            return Task.CompletedTask;
        }
    }
    
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Method)]
    public class AnonymousApiAttribute : Attribute
    {
    }

    public class AuthenticationInfo
    {
        public bool IsAuthenticated { get; private set; }

        public void SetAuthentication()
        {
            this.IsAuthenticated = true;
        }
    }

    public class UserInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
