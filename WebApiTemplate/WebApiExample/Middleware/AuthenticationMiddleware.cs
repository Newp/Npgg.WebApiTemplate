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

            bool authenticated = context.TryGetHeader("access_token", out var accessToken)
                && authenticationService.CheckAuthentication(accessToken);

            if (authenticated) return Task.CompletedTask;
            
            throw new HandledException(HttpStatusCode.Unauthorized, "you shall not pass");
        }
    }
    
    public class AnonymousApiAttribute : Attribute
    {
    }
}
