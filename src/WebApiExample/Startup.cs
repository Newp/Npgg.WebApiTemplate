using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApiExample.Middleware;
using WebApiExample.Service;
using Microsoft.EntityFrameworkCore;


namespace WebApiExample
{
    public class Startup
    {

        public virtual void ConfigureServices(IServiceCollection services)
        {
            
            services.AddControllers()
                .AddNewtonsoftJson(); // action result 의 값을 json 타입으로 변환시켜줍니다.

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
            services.AddDbContextPool<SqlContext>(option => option.UseMySQL("Server=localhost;Database=npgg;User Id=root;Password=unit_test_password;"));
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

            #region middleware , !!순서 매우 중요함!!

            app.UseMiddleware<ApiLoggingMiddleware>();
            app.UseMiddleware<TryCatchMiddleware>();
            app.UseMiddleware<AuthenticationMiddleware>();
            app.UseMiddleware<IdempotentMiddleware>();
            app.UseMiddleware<AutholizationMiddleware>();
            app.UseMiddleware<BufferMiddleware>();

            #endregion

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
