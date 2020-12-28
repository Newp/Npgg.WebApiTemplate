using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgg.Middleware;
using System.Net;
using WebApiExample.Service;

namespace WebApiExample.Middleware
{
    public class AuthenticationMiddleware : MetaDataMiddleware<AnonymousApiAttribute>, IMiddleware
    {
        private readonly AuthenticationService authenticationService;

        public AuthenticationMiddleware(AuthenticationService authenticationService) : base()
        {
            this.authenticationService = authenticationService;
        }

        public override Task Run(HttpContext context, AnonymousApiAttribute metaData)
        {
            if (metaData != null) return Task.CompletedTask;

            if(context.TryGetHeader("access_token", out var accessToken) == false
                ||  authenticationService.CheckAuthentication(accessToken, out var token) == false)
            {
                throw new HandledException(HttpStatusCode.Unauthorized, "you shall not pass");
            }

            context.SetItem(token);
            return Task.CompletedTask;
        }
    }
    
    public class AnonymousApiAttribute : Attribute
    {
    }
}
