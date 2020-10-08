using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.IO;
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

        public async Task<ApiInvokeResult> Invoke(HttpContext context, RequestDelegate next)
        {
            Stopwatch watch = new Stopwatch()

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
    }
}
