﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Npgg.Middleware;
using System.IO;
using Newtonsoft.Json;

namespace WebApiExample.Middleware
{
    public class BufferMiddleware : IMiddleware
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
            try
            {
                await next(context);
            }
            catch (HandledException hex)
            {
                context.Response.StatusCode = (int)hex.StatusCode;

                if (hex.ResponseObject != null)
                {
                    var handledExceptionBody = JsonConvert.SerializeObject(hex.ResponseObject);
                    await context.Response.WriteAsync(handledExceptionBody);
                }
            }
            finally
            {
                responseBuffer.Position = 0;
                await responseBuffer.CopyToAsync(clientResponseStream);

                context.SetItem(new RequestResponseResult(context.Response.StatusCode, requestBuffer.ToArray(), responseBuffer.ToArray()));
            }

        }
    }

    public record RequestResponseResult(int HttpStatusCode, byte[] RequestBody, byte[] ResponseBody)
    {
        public static readonly RequestResponseResult Empty = new RequestResponseResult(0, Array.Empty<byte>(), Array.Empty<byte>());
    }

}