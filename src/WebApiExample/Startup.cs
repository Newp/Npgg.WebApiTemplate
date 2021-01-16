using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                .AddNewtonsoftJson(); // action result �� ���� json Ÿ������ ��ȯ�����ݴϴ�.

            foreach(var middleware in 
                this.GetType()
                .Assembly.GetTypes()
                .Where(type => typeof(IMiddleware).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract))
            {
                services.AddScoped(middleware);
            }

            services.AddSingleton<LogService>();
            services.AddSingleton<AuthenticationService>();
            services.AddSingleton<AutholizationService>();
            services.AddSingleton<IdempotentService>();

            services.AddScoped<AuthenticationInfo>();
            
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


            
            
            app.UseMiddleware<ApiLoggingMiddleware>();
            app.UseMiddleware<TryCatchMiddleware>();
            app.UseMiddleware<AuthenticationMiddleware>();
            app.UseMiddleware<IdempotentMiddleware>();
            app.UseMiddleware<AutholizationMiddleware>();
            app.UseMiddleware<BufferMiddleware>();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}