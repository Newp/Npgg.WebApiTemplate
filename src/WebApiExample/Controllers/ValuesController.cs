using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiExample.Middleware;

namespace WebApiExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        [AnonymousApi]
        public string Get()
        {
            return "values ok";
        }


        [HttpGet("error")]
        [AnonymousApi]
        public string GetError()
        {
            throw new Exception("test error");
        }

        [HttpGet("{id}")]
        [AnonymousApi]
        public string Get(int id, [FromQuery] string? pass) //nullable 상태로 안할때 RequestQuery 
        {
            return $"value>{id}, pass:{pass}";
        }


        [HttpGet("authenticated")]
        public string AuthenticatedGet()
        {
            return "authenticated ok";
        }



        [HttpGet("autholized")]
        [Autholize(AutholizeType.Subscriber)]
        public string AutholizedGet()
        {
            return "autholized ok";
        }



        [HttpPost("exception")]
        [AnonymousApi]
        public string GetException([FromBody] string message) => throw new Exception(message);

        public static int PostCount { get; set; }

        [HttpPost]
        public async Task<int> Post([FromQuery] int delay = 0)
        {
            await Task.Delay(delay);
            return ++PostCount;
        }
    }
}
