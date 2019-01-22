using Armadillo.Siebel;
using Microsoft.AspNetCore.Blazor.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System;
using System.Net.Mime;
using Microsoft.Azure.Documents.Client;

namespace Armadillo.Server
{
    public class Startup
    {
        private ILoggerFactory loggerFactory_;
        private IConfiguration configuration_;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            loggerFactory_ = loggerFactory;
            configuration_ = configuration;
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

            // var dataProviderCache = new DataProdiverCache(
            //    new ReportServerDataProvider(loggerFactory_.CreateLogger("ReportServerDataProvider")), 
            //    loggerFactory_.CreateLogger("DataProdiverCache"), TimeSpan.FromMinutes(30));
            // services.AddSingleton<ISubcaseDataProdiver>(dataProviderCache);

            //services.AddSingleton<ISubcaseDataProdiver, RandomDataProvider>();
            
            var endpointUri = configuration_["CosmosDB:EndpointUri"];
            var primaryKey = configuration_["CosmosDB:PrimaryKey"];
            Console.WriteLine("Database endpoint {0}, key {1}", endpointUri, primaryKey);
            var documentClient = new DocumentClient(new Uri(endpointUri), primaryKey);
            var cosmosDataProvider = new CosmosDataProvider(documentClient);
            services.AddSingleton<ISubcaseDataProdiver>(cosmosDataProvider);
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
