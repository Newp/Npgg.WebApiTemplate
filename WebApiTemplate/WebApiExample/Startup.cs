using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using WebApiExample.Middleware;

namespace WebApiExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddScoped<ApiLogMiddleware>();
            services.AddSingleton<LogService>();
            services.AddSingleton<TimeService>();
            services.AddScoped<IQueryCollection, IQq>();

            //            var json = JsonSerializer.Serialize(services.ToArray());
            //var allkeys = services.ToArray().Select(item => item.Name).ToArray();
            //var xx = allkeys.Where(name => name.Contains("Value")).ToArray();
        }

        class IQq : IQueryCollection
        {
            public IQq()
            {

            }

            public StringValues this[string key] => throw new NotImplementedException();

            public int Count => throw new NotImplementedException();

            public ICollection<string> Keys => throw new NotImplementedException();

            public bool ContainsKey(string key)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public bool TryGetValue(string key, out StringValues value)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<ApiLogMiddleware>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
