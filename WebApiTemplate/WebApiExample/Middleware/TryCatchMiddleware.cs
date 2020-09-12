using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgg.Middleware;
using System.Net;
using Newtonsoft.Json;

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
                
                if(hex.ResponseObject != null)
                {
                    var handledExceptionBody = JsonConvert.SerializeObject(hex.ResponseObject);
                    await context.Response.WriteAsync(handledExceptionBody);
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 503;//internal server error
                //logger.WriteError
            }
        }
    }

    public class HandledException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public object ResponseObject { get; }
        public HandledException(HttpStatusCode statusCode, object response)
        {
            this.StatusCode = statusCode;
            this.ResponseObject = response;
        }

        
    }
}
