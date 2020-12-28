using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Npgg.Middleware
{

    public abstract class MetaDataMiddleware<T> : IMiddleware 
        where T : Attribute
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var endpoint = context.GetEndpoint();

            var metaData = endpoint.Metadata.GetMetadata<T>();

            await this.Run(context, metaData);

            await next(context);
        }

        public abstract Task Run(HttpContext context, T metaData);
    }
}
