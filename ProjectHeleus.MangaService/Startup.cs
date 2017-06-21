namespace ProjectHeleus.MangaService
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;
    using StructureMap;

    using Parsers;
    using Providers;
    using Parsers.Interfaces;
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
            services
                .AddMvc()
                .AddJsonOptions(settings => settings.SerializerSettings.Formatting = Formatting.Indented);

            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = @"Data Source=.;Initial Catalog=Cache;Integrated Security=True;";
                options.SchemaName = "dbo";
                options.TableName = "MangaImagesCache";
            });

            var container = new Container();

            container.Configure(
                config =>
                {
                    config.ForSingletonOf<ICatalogsProvider>().Add<HubCatalogsProvider>();
                    config.ForSingletonOf<IMangasProvider>().Add<HubMangasProvider>();
                    config.ForSingletonOf<IGenresProvider>().Add<HubGenresProvider>();

                    config.For<IParser>().Add<MangaFoxParser>().Named(nameof(MangaFoxParser));
                    config.For<IParser>().Add<ReadMangaParser>().Named(nameof(ReadMangaParser));
                    config.For<IParser>().Add<MintMangaParser>().Named(nameof(MintMangaParser));

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
