using Armadillo.Siebel;
using Microsoft.AspNetCore.Blazor.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System;
using System.Net.Mime;

namespace Armadillo.Server
{
    public class Startup
    {
        private ILoggerFactory loggerFactory_;

        public Startup(ILoggerFactory loggerFactory)
        {
            loggerFactory_ = loggerFactory;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    MediaTypeNames.Application.Octet,
                    WasmMediaTypeNames.Application.Wasm,
                });
            });

//            services.AddSingleton<ISubcaseDataProdiver, RandomDataProvider>();
            
            var dataProviderCache = new DataProdiverCache(
                new ReportServerDataProvider(loggerFactory_.CreateLogger("ReportServerDataProvider")), 
                loggerFactory_.CreateLogger("DataProdiverCache"), TimeSpan.FromMinutes(30));
            services.AddSingleton<ISubcaseDataProdiver>(dataProviderCache);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller}/{action}/{id?}");
            });

            app.UseBlazor<Client.Startup>();
        }
    }
}
