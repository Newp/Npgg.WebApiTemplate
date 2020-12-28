using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgg.Middleware;
using System.Net;
using WebApiExample.Service;

namespace WebApiExample.Middleware
{
    public class IdempotentMiddleware : MetaDataMiddleware<UseIdempotent>
    {
        private readonly IdempotentService idempotentService;

        public IdempotentMiddleware(IdempotentService idempotentService)
        {
            this.idempotentService = idempotentService;
        }
        
        public override async Task Run(HttpContext context, UseIdempotent metaData)
        {
            //post는 기본적으로 활성화, 그 외의 method 들은 UseIdempotent 메타데이터의 유무로 사용
            const string postMethod = "post";
            if (context.Request.Method.ToLower() == postMethod || metaData != null)
            {
                return;
            }

            if ( context.TryGetHeader("request-id", out var requestId) == false)
            {
                throw new HandledException(HttpStatusCode.BadRequest, "idempotent request need 'request-id'");
            }

        }
    }
}
