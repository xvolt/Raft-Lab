using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RaftCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NodeConsole
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore().AddJsonFormatters();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc(x => x.MapRoute("Default", "{controller}/{action}"));
        }
    }
}
