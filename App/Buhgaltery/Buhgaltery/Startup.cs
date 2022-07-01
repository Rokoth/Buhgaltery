using AutoMapper;
using Buhgaltery.BuhgalteryDeployer;
using Buhgaltery.Common;
using Buhgaltery.Db.Context;
using Buhgaltery.Db.Interface;
using Buhgaltery.Db.Repository;
using Buhgaltery.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;

namespace Buhgaltery
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
            services.Configure<CommonOptions>(Configuration);
            services.AddControllersWithViews();
            services.AddLogging();
            services.AddSingleton<IErrorNotifyService, ErrorNotifyService>();
            services.AddDbContextPool<DbPgContext>((opt) =>
            {
                opt.EnableSensitiveDataLogging();
                var connectionString = Configuration.GetConnectionString("MainConnection");
                opt.UseNpgsql(connectionString);
            });

            services.AddCors();
            services.AddAuthentication()
            .AddJwtBearer("Token", (options) =>
            {
                AuthOptions settings = Configuration.GetSection("AuthOptions").Get<AuthOptions>();
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //// укзывает, будет ли валидироваться издатель при валидации токена
                    ValidateIssuer = true,
                    //// строка, представляющая издателя
                    ValidIssuer = settings.Issuer,

                    //// будет ли валидироваться потребитель токена
                    ValidateAudience = true,
                    //// установка потребителя токена
                    ValidAudience = settings.Audience,
                    //// будет ли валидироваться время существования
                    ValidateLifetime = true,

                    // установка ключа безопасности
                    IssuerSigningKey = settings.GetSymmetricSecurityKey(),
                    // валидация ключа безопасности
                    ValidateIssuerSigningKey = true,

                };
            });

            services
                .AddAuthorization(options =>
                {
                    var defPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes("Token")
                        .Build();                   
                    options.AddPolicy("Token", defPolicy);
                    options.DefaultPolicy = defPolicy;
                });

            services.AddScoped<IRepository<Db.Model.User>, Repository<Db.Model.User>>();         
            services.AddScoped<IRepository<Db.Model.UserHistory>, Repository<Db.Model.UserHistory>>();
            services.AddScoped<IRepository<Db.Model.UserSettings>, Repository<Db.Model.UserSettings>>();
            services.AddScoped<IRepository<Db.Model.UserSettingsHistory>, Repository<Db.Model.UserSettingsHistory>>();
            services.AddScoped<IRepository<Db.Model.Formula>, Repository<Db.Model.Formula>>();
            services.AddScoped<IRepository<Db.Model.FormulaHistory>, Repository<Db.Model.FormulaHistory>>();
            services.AddScoped<IRepository<Db.Model.Product>, Repository<Db.Model.Product>>();
            services.AddScoped<IRepository<Db.Model.ProductHistory>, Repository<Db.Model.ProductHistory>>();
            services.AddScoped<IRepository<Db.Model.Incoming>, Repository<Db.Model.Incoming>>();
            services.AddScoped<IRepository<Db.Model.IncomingHistory>, Repository<Db.Model.IncomingHistory>>();
            services.AddScoped<IRepository<Db.Model.Outgoing>, Repository<Db.Model.Outgoing>>();
            services.AddScoped<IRepository<Db.Model.OutgoingHistory>, Repository<Db.Model.OutgoingHistory>>();
            services.AddScoped<IRepository<Db.Model.Reserve>, Repository<Db.Model.Reserve>>();
            services.AddScoped<IRepository<Db.Model.ReserveHistory>, Repository<Db.Model.ReserveHistory>>();
            services.AddScoped<IRepository<Db.Model.Correction>, Repository<Db.Model.Correction>>();
            services.AddScoped<IRepository<Db.Model.CorrectionHistory>, Repository<Db.Model.CorrectionHistory>>();

            services.AddScoped<IAllocateReservesService, AllocateReservesService>();
            services.AddScoped<IReservesRevisorService, ReservesRevisorService>();

            services.AddHostedService<ReservesRevisorHostedService>();
            services.AddHostedService<AllocateReservesHostedService>();

            services.AddDataServices();

            services.AddScoped<IDeployService, DeployService>();
            services.AddScoped<ICalculator, CalculatorNCalc>();
            services.ConfigureAutoMapper();
            services.AddSwaggerGen(swagger =>
            {
                //s.OperationFilter<AddRequiredHeaderParameter>();

                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                    }
                });
            });

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
            app.UseAuthorization();
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.Options.SourcePath = "..\\..\\..\\ClientApp";
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });            
        }
    }

    public class AddRequiredHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Description = "access token",
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new OpenApiString("Bearer ")
                }
            });
        }
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Db.Model.User, Contract.Model.User>();
            CreateMap<Contract.Model.UserCreator, Db.Model.User>()
                .ForMember(s => s.Password, s => s.Ignore());
            CreateMap<Db.Model.UserHistory, Contract.Model.UserHistory>();
            CreateMap<Contract.Model.UserUpdater, Db.Model.User>()
                .ForMember(s => s.Password, s => s.Ignore());

            CreateMap<Db.Model.Formula, Contract.Model.Formula>();
            CreateMap<Contract.Model.FormulaCreator, Db.Model.Formula>();
            CreateMap<Db.Model.FormulaHistory, Contract.Model.FormulaHistory>();
            CreateMap<Contract.Model.FormulaUpdater, Db.Model.Formula>();

            CreateMap<Db.Model.Product, Contract.Model.Product>();
            CreateMap<Contract.Model.ProductCreator, Db.Model.Product>();
            CreateMap<Db.Model.ProductHistory, Contract.Model.ProductHistory>();
            CreateMap<Contract.Model.ProductUpdater, Db.Model.Product>();

            CreateMap<Db.Model.Incoming, Contract.Model.Incoming>();
            CreateMap<Contract.Model.IncomingCreator, Db.Model.Incoming>();
            CreateMap<Db.Model.IncomingHistory, Contract.Model.IncomingHistory>();
            CreateMap<Contract.Model.IncomingUpdater, Db.Model.Incoming>();

            CreateMap<Db.Model.Outgoing, Contract.Model.Outgoing>();
            CreateMap<Contract.Model.OutgoingCreator, Db.Model.Outgoing>();
            CreateMap<Db.Model.OutgoingHistory, Contract.Model.OutgoingHistory>();
            CreateMap<Contract.Model.OutgoingUpdater, Db.Model.Outgoing>();

            CreateMap<Db.Model.Reserve, Contract.Model.Reserve>();
            CreateMap<Contract.Model.ReserveCreator, Db.Model.Reserve>();
            CreateMap<Db.Model.ReserveHistory, Contract.Model.ReserveHistory>();
            CreateMap<Contract.Model.ReserveUpdater, Db.Model.Reserve>();

            CreateMap<Db.Model.Correction, Contract.Model.Correction>();
            CreateMap<Contract.Model.CorrectionCreator, Db.Model.Correction>();
            CreateMap<Db.Model.CorrectionHistory, Contract.Model.CorrectionHistory>();
            CreateMap<Contract.Model.CorrectionUpdater, Db.Model.Correction>();
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
