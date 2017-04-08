using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;
using HeartbeatService.Models;
using System.Net.Http;
using HeartbeatService.Controllers;

namespace HeartbeatService
{
    public class Startup
    {
        private Timer _timer;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.Configure<MongoOptions>(Configuration.GetSection("AppSettings").GetSection("Mongo"));
            services.AddSingleton<IEndpointRepository, MongoDbEndpointRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IEndpointRepository repository)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            
            var timerPeriod = Int32.Parse(Configuration.GetSection("AppSettings").GetSection("TimerPeriod").Value);

            _timer = new Timer(new TimerCallback(s =>
            {
                var heartbeats = repository.GetAll();

                var httpClient = new HttpClient();
                foreach (var heartbeat in heartbeats)
                {
                    try
                    {
                        httpClient.GetAsync(heartbeat.Url);
                    }
                    catch (Exception) { }
                }

                //invoke himself
                var controller = new HomeController(repository);
                controller.Index();
            }),
            null,
            0,
            timerPeriod);
        }
    }
}
