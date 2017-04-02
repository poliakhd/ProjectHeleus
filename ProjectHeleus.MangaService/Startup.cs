using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectHeleus.MangaService.Parsers;
using ProjectHeleus.MangaService.Parsers.Contracts;
using ProjectHeleus.MangaService.Providers;
using ProjectHeleus.MangaService.Providers.Contracts;
using StructureMap;

namespace ProjectHeleus.MangaService
{
    public class Startup
    {
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

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var container = new Container();

            container.Configure(
                config =>
                {
                    config.For<ICatalogsProvider>().Add<HubCatalogsProvider>();
                    config.For<IMangaProvider>().Add<HubMangaProvider>();

                    config.For<IParser>().Add<MangaFoxParser>().Named(nameof(MangaFoxParser));
                    config.For<IParser>().Add<ReadMangaParcer>().Named(nameof(ReadMangaParcer));

                    config.Populate(services);
                }
            );

            return container.GetInstance<IServiceProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
