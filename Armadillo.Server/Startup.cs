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
using System.Net.Http;
using System.Net;
using Microsoft.Azure.Documents.Client;
using Armadillo.Data;

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

            // Configure ReportServerClient powered by HttpClientHandler with NTLM authentication
            var credentials = new CredentialCache { { new Uri(ReportServerDataProvider.ReportServerUrl), "NTLM", CredentialCache.DefaultNetworkCredentials } };
            var clientHandler = new HttpClientHandler { Credentials = credentials };
            services
                .AddHttpClient<IReportServerClient, ReportServerClient>()
                .ConfigurePrimaryHttpMessageHandler(() => clientHandler);

            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    MediaTypeNames.Application.Octet,
                    WasmMediaTypeNames.Application.Wasm,
                });
            });

            AddDataProvider(services);
        }

        private void AddDataProvider(IServiceCollection services)
        {
            var logger = loggerFactory_.CreateLogger("Startup");

            var dataProviderName = configuration_["SubcaseDataProvider"];
            logger.LogInformation($"Data provider: {dataProviderName}");

            if(String.Equals("Random", dataProviderName, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogInformation("Using random data provider");
                services.AddSingleton<ISubcaseDataProdiver, RandomDataProvider>();
            }
            else if(String.Equals("Report", dataProviderName, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogInformation("Using SSRS report data provider");
                var serviceProvider = services.BuildServiceProvider();
                var reportServerClient = serviceProvider.GetService<IReportServerClient>();

                var dataProviderCache = new DataProdiverCache(
                    new ReportServerDataProvider(loggerFactory_.CreateLogger("ReportServerDataProvider"), reportServerClient), 
                    loggerFactory_.CreateLogger("DataProdiverCache"), TimeSpan.FromMinutes(5));
                
                services.AddSingleton<ISubcaseDataProdiver>(dataProviderCache);
            }
            else if(String.Equals("Cosmos", dataProviderName, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogInformation("Using CosmosDB data provider");

                var endpointUri = configuration_["CosmosDB:EndpointUri"];
                var primaryKey = configuration_["CosmosDB:PrimaryKey"];
                
                logger.LogDebug($"Database endpoint {endpointUri}");
                var documentClient = new DocumentClient(new Uri(endpointUri), primaryKey);

                var dataProviderCache = new DataProdiverCache(
                    new CosmosDataProvider(documentClient, loggerFactory_.CreateLogger("CosmosDataProvider")),
                    loggerFactory_.CreateLogger("DataProdiverCache"), TimeSpan.FromMinutes(1));

                services.AddSingleton<ISubcaseDataProdiver>(dataProviderCache);
            }
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
