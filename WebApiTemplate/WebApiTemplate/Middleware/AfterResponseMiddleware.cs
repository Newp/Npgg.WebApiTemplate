using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npgg.Middleware
{
    public abstract class AfterResponseMiddleware : IMiddleware
    {
        Stopwatch watch = new Stopwatch();

        public abstract void AfterResponse(HttpContext context, byte[] requestBody, byte[] responseBody, long elapsedMilliseconds);

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            watch.Restart();

            var endpoint = context.GetEndpoint();

            byte[] request = null;
            byte[] response = null;
            
            context.Request.EnableBuffering();

            //request scope
            var requestBuffer = new MemoryStream();
            await context.Request.BodyReader.CopyToAsync(requestBuffer);
            context.Request.Body.Position = 0;
            request = requestBuffer.ToArray();
                
            //response scope
            var clientResponseStream = context.Response.Body;
            var responseBuffer = new MemoryStream();
            context.Response.Body = responseBuffer;

            //process api action
            await next(context);
            response = responseBuffer.ToArray();
            await clientResponseStream.WriteAsync(response, 0, response.Length);

            watch.Stop();
            
            this.AfterResponse(context, request, response, watch.ElapsedMilliseconds);
        }
    }
}
