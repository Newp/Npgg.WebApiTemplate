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

namespace WebApiExample.Middleware
{
    public class ApiLogMiddleware : AfterResponseMiddleware
    {
        static readonly UTF8Encoding encoding = new UTF8Encoding(false);
        private readonly LogService logger;

        public ApiLogMiddleware( LogService logger)
        {
            this.logger = logger;
        }

        public override void AfterResponse(HttpContext context, byte[] requestBody, byte[] responseBody, long elapsedMilliseconds)
        {
            var endpoint = context.GetEndpoint();

            
            //var metadatas = endpoint.Metadata.ToArray();
            //var method = endpoint.Metadata.GetMetadata<HttpMethodAttribute>();
            //var controller = endpoint.Metadata.GetMetadata<RouteAttribute>();

            var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
            
            var log = new ApiLog()
            {
                Elapsed = elapsedMilliseconds,
                ApiPattern = actionDescriptor?.AttributeRouteInfo.Template,
                Request = new RequestLog()
                {
                    Path = context.Request.Path.Value,
                    Body = encoding.GetString(requestBody),
                    Headers = JsonSerializer.Serialize( context.Request.Headers),
                    QueryString = context.Request.QueryString.ToString(),
                    Method = context.Request.Method.ToLower()
                },
                Response = new ResponseLog()
                {
                    Body = encoding.GetString(responseBody),
                    Headers = JsonSerializer.Serialize(context.Response.Headers),
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