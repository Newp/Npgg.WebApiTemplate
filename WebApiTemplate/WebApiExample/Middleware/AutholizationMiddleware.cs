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
    public class AutholizationMiddleware : MetaDataMiddleware<AutholizeAttribute>
    {
        private readonly AutholizationService autholizationService;

        public AutholizationMiddleware(AutholizationService autholizationService)
        {
            this.autholizationService = autholizationService;
        }

        public override Task Run(HttpContext context, AutholizeAttribute metaData)
        {
            if (metaData == null)
                return Task.CompletedTask;

            var accessToken = context.GetItem<AccessToken>();

            if (this.autholizationService.CheckAutholize(accessToken, metaData.Autholize) == false)
            {
                throw new HandledException(HttpStatusCode.Forbidden);
            }

            return Task.CompletedTask;
        }
    }

    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Method)]
    public class AutholizeAttribute : Attribute
    {
        public AutholizeAttribute(AutholizeType autholize)
        {
            Autholize = autholize;
        }

        public AutholizeType Autholize { get; }
    }
    
}
