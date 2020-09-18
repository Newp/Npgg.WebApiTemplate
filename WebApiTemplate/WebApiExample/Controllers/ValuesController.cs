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
        public string Get(int id, [FromQuery] string pass)
        {
            return $"value>{id}, pass:{pass}";
        }


        [HttpGet("authenticated")]
        public string AuthenticatedGet()
        {
            return "authenticated ok";
        }



    }
}
