using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgg.Middleware;
using System.Net;

namespace WebApiExample.Middleware
{
    public class TryCatchMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var endpoint = context.GetEndpoint();

            if (endpoint == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }


            try
            {
                
                await next(context);
            }
            catch (HandledException hex)
            {
                context.Response.StatusCode = (int)hex.StatusCode;
                
            }
            catch (Exception ex)
            {
                //throw;
            }
        }
    }

    public class HandledException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public HandledException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public string Messag { get; set; }

    }
}
