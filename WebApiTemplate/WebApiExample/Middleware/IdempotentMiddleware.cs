﻿using Microsoft.AspNetCore.Http;
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

            var cached = idempotentService.Get(requestId) as RequestResponseBody;

            if(cached != null)
            {
                context.Response.StatusCode = cached.HttpStatusCode;
                await context.Response.Body.WriteAsync(cached.ResponseBody);
                return;
            }

            await next(context);

            var proceed = context.GetItem<RequestResponseBody>();

            idempotentService.Set(requestId, proceed);
        }
    }
}
