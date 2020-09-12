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
    public class AuthenticationMiddleware : IMiddleware
    {
        private readonly AuthenticationService authenticationService;

        public AuthenticationMiddleware(AuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var endpoint = context.GetEndpoint();

            bool isAnonymous = endpoint.Metadata.GetMetadata<AnonymousApiAttribute>() != null;

            bool authenticated = context.Request.Headers.TryGetValue<string>("access_token", out var accessToken)
                && authenticationService.CheckAuthentication(accessToken);

            if (isAnonymous == false && authenticated == false)
            {
                throw new HandledException(HttpStatusCode.Unauthorized, "you shall not pass");
            }

            await next(context);
        }

    }
    
    public class AnonymousApiAttribute : Attribute
    {
    }
}
