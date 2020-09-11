using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npgg.Middleware
{

    public struct ApiInvokeResult
    {
        public byte[] RequestBody;
        public byte[] ResponseBody;
        public long ElapsedMilliseconds;
    }

    public abstract class TopLevelMiddleware
    {
        readonly Stopwatch watch = new Stopwatch();

        public async Task<ApiInvokeResult> Invoke(HttpContext context, RequestDelegate next)
        {
            watch.Restart();

            context.Request.EnableBuffering();

            //request scope
            var requestBuffer = new MemoryStream();
            await context.Request.BodyReader.CopyToAsync(requestBuffer);
            context.Request.Body.Position = 0;

            //response scope
            var clientResponseStream = context.Response.Body;
            var responseBuffer = new MemoryStream();
            context.Response.Body = responseBuffer;

            //process api action
            await next(context);

            responseBuffer.Position = 0;
            await responseBuffer.CopyToAsync(clientResponseStream);

            //await clientResponseStream.WriteAsync(response, 0, response.Length);

            watch.Stop();

            return new ApiInvokeResult()
            {
                ElapsedMilliseconds = watch.ElapsedMilliseconds,
                RequestBody = requestBuffer.ToArray(),
                ResponseBody = responseBuffer.ToArray(),
            };
        }

        //public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        //{
        //    watch.Restart();

        //    byte[] request, response;
            
        //    context.Request.EnableBuffering();

        //    //request scope
        //    var requestBuffer = new MemoryStream();
        //    await context.Request.BodyReader.CopyToAsync(requestBuffer);
        //    context.Request.Body.Position = 0;
        //    request = requestBuffer.ToArray();
                
        //    //response scope
        //    var clientResponseStream = context.Response.Body;
        //    var responseBuffer = new MemoryStream();
        //    context.Response.Body = responseBuffer;

        //    //process api action
        //    await next(context);
        //    response = responseBuffer.ToArray();
        //    await clientResponseStream.WriteAsync(response, 0, response.Length);

        //    watch.Stop();
            
        //    this.AfterResponse(context, request, response, watch.ElapsedMilliseconds);
        //}
    }
}
