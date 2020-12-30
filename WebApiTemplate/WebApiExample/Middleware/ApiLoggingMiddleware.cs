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
using System.Runtime.Serialization;

namespace WebApiExample.Middleware
{
    public struct ApiInvokeResult
    {
        public byte[] RequestBody;
        public byte[] ResponseBody;
        public long ElapsedMilliseconds;
    }

    public class ApiLoggingMiddleware :  IMiddleware
    {
        static readonly UTF8Encoding encoding = new UTF8Encoding(false);
        private readonly LogService logger;
        
        public ApiLoggingMiddleware(LogService logger)
        {
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Stopwatch watch = new Stopwatch();

            watch.Restart();

            await next(context);

            watch.Stop();

            var endpoint = context.GetEndpoint();
            var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
            var buffer = context.GetItem<RequestResponseResult>() ?? RequestResponseResult.Empty;

            var log = new ApiLog
            (
                Elapsed: watch.ElapsedMilliseconds,
                ApiPattern: actionDescriptor?.AttributeRouteInfo?.Template,
                new RequestLog(
                    Path : context.Request.Path.Value,
                    Method: encoding.GetString(buffer.RequestBody),
                    Headers: JsonConvert.SerializeObject(context.Request.Headers),
                    QueryString: context.Request.QueryString.ToString(),
                    context.Request.Method.ToLower()
                ),
                new ResponseLog(
                    context.Response.StatusCode,
                    JsonConvert.SerializeObject(context.Response.Headers),
                    encoding.GetString(buffer.ResponseBody)
                )
            );

            logger.Write("api_result", log);
        }
    }

    public record ApiLog(long Elapsed, string ApiPattern, RequestLog Request, ResponseLog Response);

    public record RequestLog
    (
        string Path ,
        string Method ,
        string Headers ,
        string QueryString ,
        string Body
    );


    public record ResponseLog
    (
        [property : JsonProperty("status")] int Status ,
        [property : JsonProperty("headers")] string Headers ,
        [property : JsonProperty("body")] string Body
    );
}