
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;

namespace WebApiExample
{
    public class SqlContext :DbContext
    {
        public SqlContext(DbContextOptions options) : base(options)
        {

        }
    }

}