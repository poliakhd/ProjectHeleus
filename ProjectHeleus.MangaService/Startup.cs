using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectHeleus.MangaService.Parsers;
using ProjectHeleus.MangaService.Parsers.Interfaces;
using ProjectHeleus.MangaService.Providers;
using StructureMap;

namespace ProjectHeleus.MangaService
{
    using Providers.Interfaces;

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
                    config.ForSingletonOf<ICatalogsProvider>().Add<HubCatalogsProvider>();
                    config.ForSingletonOf<IMangasProvider>().Add<HubMangasProvider>();
                    config.ForSingletonOf<IGenresProvider>().Add<HubGenresProvider>();

                    config.ForSingletonOf<IParser>().Add<MangaFoxParser>().Named(nameof(MangaFoxParser));
                    config.ForSingletonOf<IParser>().Add<ReadMangaParser>().Named(nameof(ReadMangaParser));
                    config.ForSingletonOf<IParser>().Add<MintMangaParser>().Named(nameof(MintMangaParser));

                    config.Populate(services);
                }
            );

            return container.GetInstance<IServiceProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddFile("Logs/{Date}.log");

            app.UseMvc();
        }
    }
}
