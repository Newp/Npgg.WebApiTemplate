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
    public class IdempotentMiddleware :IMiddleware
    {
        private readonly IdempotentService idempotentService;

        public IdempotentMiddleware(IdempotentService idempotentService)
        {
            this.idempotentService = idempotentService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            const string postMethod = "post";
            var useIdempotent = context.Request.Method.ToLower() == postMethod ||
                context.GetEndpoint()?.Metadata.GetMetadata<UseIdempotent>() != null;
            
            if (useIdempotent == false)
            {
                await next(context);
                return;
            }

            if (context.TryGetHeader("request-id", out var requestId) == false)
            {
                throw new HandledException(HttpStatusCode.BadRequest, "idempotent request need 'request-id'");
            }

            if (idempotentService.Get(requestId) is RequestResponseResult cached)
            {
                context.Response.StatusCode = cached.HttpStatusCode;
                await context.Response.Body.WriteAsync(cached.ResponseBody);
                return;
            }


            if(idempotentService.TryPreoccupy(requestId) == false)
            //이미 다른 request, 가 권한을 선점했다. indentical request 
            {
                throw new HandledException(HttpStatusCode.Conflict);
            }

            await next(context);

            idempotentService.ReleaseAcquire(requestId);

            var proceed = context.GetRequiredItem<RequestResponseResult>();

            idempotentService.Set(requestId, proceed);
        }
    }
}
