using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApiExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        [Description("test gogogo")]
        public string Get()
        {
            return "values ok";
        }

        [HttpGet("{id}")]
        [Description("test id get")]
        public string Get(int id, string pass)
        {
            return "value>" + id;
        }


    }
}
