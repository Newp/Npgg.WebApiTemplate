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
using WebApiExample.Service;

namespace WebApiExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(); // action result 의 값을 json 타입으로 변환시켜줍니다.

            foreach(var middleware in 
                this.GetType()
                .Assembly.GetTypes()
                .Where(type => typeof(IMiddleware).IsAssignableFrom(type) && type.IsClass))
            {
                services.AddScoped(middleware);
            }

            services.AddSingleton<LogService>();
            services.AddSingleton<TimeService>();
            services.AddSingleton<AuthenticationService>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();


            
            
            app.UseMiddleware<TopLevelMiddleware>();
            app.UseMiddleware<TryCatchMiddleware>();
            app.UseMiddleware<AuthenticationMiddleware>();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
