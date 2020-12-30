using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Npgg.Middleware;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Text.Json;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

namespace WebApiExample.Middleware
{
    public class BufferMiddleware :  IMiddleware
    {
        

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            context.Request.EnableBuffering();

            //request scope
            using var requestBuffer = new MemoryStream();
            await context.Request.BodyReader.CopyToAsync(requestBuffer);
            context.Request.Body.Position = 0;

            //response scope
            var clientResponseStream = context.Response.Body;
            using var responseBuffer = new MemoryStream();
            context.Response.Body = responseBuffer;

            //process api action
            await next(context);

            responseBuffer.Position = 0;
            await responseBuffer.CopyToAsync(clientResponseStream);

            context.SetItem(new RequestResponseBody()
            {
                HttpStatusCode = context.Response.StatusCode,
                RequestBody = requestBuffer.ToArray(),
                ResponseBody = responseBuffer.ToArray(),
            }); 
        }
    }

    public class RequestResponseBody
    {
        public static readonly RequestResponseBody Empty = new RequestResponseBody()
        {
            HttpStatusCode = 0,
            RequestBody = Array.Empty<byte>(),
            ResponseBody = Array.Empty<byte>(),
        };

        public int HttpStatusCode { get; set; }
        public byte[] RequestBody { get; set; }
        public byte[] ResponseBody { get; set; }
    }

}