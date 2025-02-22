using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VueCliMiddleware;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.AspNetCore.Mvc.Cors;
using Newtonsoft.Json;
using aspVue7.Model;
using Microsoft.AspNetCore.NodeServices;

namespace aspVue7
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
            //数据库连接语句
            BorgWarnerMisSQLContext.ConnectionString = Configuration.GetConnectionString("nbjsMESContext"); 
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);   
            services.AddControllers();
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp";
            });
            //是的cs文件中可以执行js文件
            services.AddNodeServices();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseSpaStaticFiles();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
              {
                  endpoints.MapControllerRoute(
                      name: "default",
                      pattern: "{controller=Home}/{action=Index}/{id?}");
             });

            app.UseSpa(spa =>
            {
                if (env.IsDevelopment())
                    spa.Options.SourcePath = "ClientApp";
                else
                    spa.Options.SourcePath = "dist";

                if (env.IsDevelopment())
                {
                    spa.UseVueCli(npmScript: "serve",port:8080);
                }

            });
            // app.UseMvc(routes =>
            // { 
            //     routes.MapRoute(
            //         name: "api",
            //         template: "api/{controller}/{action=Index}/{id?}");
            // });
        }
    }
}