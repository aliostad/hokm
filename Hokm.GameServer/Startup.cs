using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;

namespace Hokm.GameServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddHostedService<MatchReportHubService>();
            services.AddSingleton<IMachReportSink, WebsocketsSink>();
            services.AddSingleton<IMachReportSink, ConsoleSink>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/version", async context => { await context.Response.WriteAsync("Hello World!"); });
                endpoints.MapGet("/", httpContext =>
                {
                    httpContext.Response.Redirect("/index.html");
                    return Task.CompletedTask;
                });
                endpoints.MapHub<MatchReporterHub>("/games");
            });
        }
    }
}