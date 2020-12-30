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

            var log = new ApiLog()
            {
                Elapsed = watch.ElapsedMilliseconds,
                ApiPattern = actionDescriptor?.AttributeRouteInfo.Template,
                Request = new RequestLog()
                {
                    Path = context.Request.Path.Value,
                    Body = encoding.GetString(buffer.RequestBody),
                    Headers = JsonConvert.SerializeObject(context.Request.Headers),
                    QueryString = context.Request.QueryString.ToString(),
                    Method = context.Request.Method.ToLower()
                },
                Response = new ResponseLog()
                {
                    Body = encoding.GetString(buffer.ResponseBody),
                    Headers = JsonConvert.SerializeObject(context.Response.Headers),
                    Status = context.Response.StatusCode
                }
            };

            logger.Write("api_result", log);
        }
    }

    public class ApiLog
    {
        public long Elapsed { get;  set; }
        public string ApiPattern { get; set; }
        public RequestLog Request { get; set; }
        public ResponseLog Response { get; set; }
    }

    public class RequestLog
    {
        public string Path { get; set; }
        public string Method { get; set; }
        public string Headers { get; set; }
        public string QueryString { get; set; }
        public string Body { get; set; }
    }


    public class ResponseLog
    {
        public int Status { get; set; }
        public string Headers { get; set; }
        public string Body { get; set; }
    }
}