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
    public class AutholizationMiddleware : IMiddleware
    {
        private readonly AutholizationService autholizationService;

        public AutholizationMiddleware(AutholizationService autholizationService)
        {
            this.autholizationService = autholizationService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var endpoint = context.GetEndpoint();


            var list = endpoint.Metadata.GetOrderedMetadata<AutholizeAttribute>();

            foreach (var needAuth in Enumerable.Empty<AutholizeAttribute>())
            {
                if(this.autholizationService.CheckAutholize(needAuth.Autholize)== false)
                {

                }
            }

            await next(context);
        }

    }

    public class AutholizeAttribute : Attribute
    {
        public AutholizeAttribute(AutholizeType autholize)
        {
            Autholize = autholize;
        }

        public AutholizeType Autholize { get; }
    }
    
}
