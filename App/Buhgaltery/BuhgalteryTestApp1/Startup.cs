using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using Buhgaltery.BuhgalteryDeployer;
using Buhgaltery.Common;
using Buhgaltery.Db.Context;
using Buhgaltery.Db.Interface;
using Buhgaltery.Db.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;

namespace BuhgalteryTestApp1
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
            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<Db.Model.User, Contract.Model.User>();

            //CreateMap<Contract.Model.UserCreator, Db.Model.User>()
            //    .ForMember(s => s.Password, s => s.Ignore());

            //CreateMap<Db.Model.UserHistory, Contract.Model.UserHistory>();

            //CreateMap<Contract.Model.UserUpdater, Db.Model.User>()
            //    .ForMember(s => s.Password, s => s.Ignore());

            //CreateMap<Db.Model.Client, Contract.Model.Client>();

            //CreateMap<Contract.Model.ClientCreator, Db.Model.Client>()
            //    .ForMember(s => s.Password, s => s.Ignore());

            //CreateMap<Db.Model.ClientHistory, Contract.Model.ClientHistory>();

            //CreateMap<Contract.Model.ClientUpdater, Db.Model.Client>()
            //    .ForMember(s => s.Password, s => s.Ignore());

            //CreateMap<Db.Model.Release, Contract.Model.Release>();
            //CreateMap<Contract.Model.ReleaseCreator, Db.Model.Release>();
            //CreateMap<Contract.Model.ReleaseUpdater, Db.Model.Release>();

            //CreateMap<Db.Model.ReleaseArchitect, Contract.Model.ReleaseArchitect>();
            //CreateMap<Contract.Model.ReleaseArchitectCreator, Db.Model.ReleaseArchitect>();
            //CreateMap<Contract.Model.ReleaseArchitectUpdater, Db.Model.ReleaseArchitect>();

            //CreateMap<Db.Model.LoadHistory, Contract.Model.LoadHistory>();
            //CreateMap<Contract.Model.LoadHistoryCreator, Db.Model.LoadHistory>();
        }
    }

    public static class CustomExtensionMethods
    {
        public static IConfigurationBuilder AddDbConfiguration(this IConfigurationBuilder builder)
        {
            var configuration = builder.Build();
            var connectionString = configuration.GetConnectionString("MainConnection");
            builder.AddConfigDbProvider(options => options.UseNpgsql(connectionString), connectionString);
            return builder;
        }

        public static IConfigurationBuilder AddConfigDbProvider(
            this IConfigurationBuilder configuration, Action<DbContextOptionsBuilder> setup, string connectionString)
        {
            configuration.Add(new ConfigDbSource(setup, connectionString));
            return configuration;
        }

        public static IServiceCollection ConfigureAutoMapper(this IServiceCollection services)
        {
            var mappingConfig = new AutoMapper.MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));

            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            return services;
        }

        public static ILoggingBuilder AddErrorNotifyLogger(
        this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, ErrorNotifyLoggerProvider>());

            LoggerProviderOptions.RegisterProviderOptions
                <ErrorNotifyLoggerConfiguration, ErrorNotifyLoggerProvider>(builder.Services);

            return builder;
        }

        public static ILoggingBuilder AddErrorNotifyLogger(
            this ILoggingBuilder builder,
            Action<ErrorNotifyLoggerConfiguration> configure)
        {
            builder.Services.Configure(configure);
            builder.AddErrorNotifyLogger();

            return builder;
        }
    }
}
