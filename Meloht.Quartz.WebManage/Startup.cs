using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meloht.Quartz.JobCore;
using Meloht.Quartz.JobCore.Log;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Logging;

namespace Meloht.Quartz.WebManage
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                LogProvider.SetCurrentLogProvider(new CustomConsoleLogProvider());
            }
            else
            {
                app.UseExceptionHandler("/Job/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            Log4NetHelper.Instance.InitLog();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Job}/{action=JobListIndex}/{id?}");
            });

            JobManger.RunJob();
        }
    }
}
