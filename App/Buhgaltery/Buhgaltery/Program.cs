using Buhgaltery.BuhgalteryDeployer;
using Buhgaltery.Common;
using Buhgaltery.Db.Context;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Topshelf;

namespace Buhgaltery
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var loggerConfig = new LoggerConfiguration()
               .WriteTo.Console()
               .WriteTo.File("Logs\\log-startup.txt")
               .MinimumLevel.Verbose();

            using var logger = loggerConfig.CreateLogger();
            logger.Information($"Service starts with arguments: {string.Join(", ", args)}");

            var exitCode = HostFactory.Run(x =>
            {
                x.Service<Starter>(s =>
                {
                    s.ConstructUsing(_ => new Starter(logger, args));
                    s.WhenStarted(starter => starter.Start());
                    s.WhenStopped(starter => starter.Stop());
                });

                x.RunAsLocalService();
                x.EnableServiceRecovery(r => r.RestartService(TimeSpan.FromSeconds(10)));
                x.SetDescription($"Buhgaltery Service, 2021 (�)");
                x.SetDisplayName($"Buhgaltery Service");
                x.SetServiceName($"BuhgalteryService");
                x.StartAutomatically();
            });
            logger.Information($"Service stops with exit code: {exitCode}");
        }
    }

    public class Starter
    {
        private IWebHost webHost;
        private readonly string[] Args;
        private Serilog.ILogger _logger;

        public Starter(Serilog.ILogger logger, string[] args)
        {
            _logger = logger;
            Args = args;
        }

        public bool Start()
        {
            try
            {
                webHost = BuildWebHost(GetWebHostBuilder(Args));
                _logger.Information("Start service...");
                webHost.Start();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Starting service error! \nException:\n {ex.Message} \nStackTrace:\n {ex.StackTrace} ");
                throw;
            }
        }

        public bool Stop()
        {
            webHost?.StopAsync().ContinueWith(s =>
            {
                if (s.IsFaulted)
                {
                    _logger.Error($"Stopping service error! \nException:\n {s.Exception}");
                }
                webHost?.Dispose();
            });
            return true;
        }

        private IWebHost BuildWebHost(IWebHostBuilder webHostBuilder)
        {
            return webHostBuilder.UseStartup<Startup>()
                .Build();
        }

        protected IWebHostBuilder GetWebHostBuilder(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(
                    new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .AddDbConfiguration()
                    .Build())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .CreateLogger();
                    logging.AddSerilog(Log.Logger);
                    logging.AddErrorNotifyLogger(config=> {                        
                        var opt =  hostingContext.Configuration.GetSection("ErrorNotifyOptions").Get<ErrorNotifyOptions>();
                        config.Options = opt;
                    });
                })
                .UseKestrel();

            return builder;
        }
    }

    public class ConfigDbSource : IConfigurationSource
    {
        private readonly Action<DbContextOptionsBuilder> _optionsAction;
        private string _connectionString;

        public ConfigDbSource(Action<DbContextOptionsBuilder> optionsAction, string connectionString)
        {
            _optionsAction = optionsAction;
            _connectionString = connectionString;
        }

        public Microsoft.Extensions.Configuration.IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            IDeployService deployService = new DeployService(_connectionString);
            return new ConfigDbProvider(_optionsAction, deployService);
        }
    }

    public class ConfigDbProvider : ConfigurationProvider
    {
        private readonly Action<DbContextOptionsBuilder> _options;
        private readonly IDeployService _deployService;

        public ConfigDbProvider(Action<DbContextOptionsBuilder> options,
            IDeployService deployService)
        {
            _options = options;
            _deployService = deployService;
        }

        public override void Load()
        {
            try
            {
                LoadInternal();
            }
            catch
            {
                try
                {
                    _deployService.Deploy().Wait();
                    LoadInternal();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void LoadInternal()
        {
            var builder = new DbContextOptionsBuilder<DbPgContext>();
            _options(builder);

            using (var context = new DbPgContext(builder.Options))
            {
                var items = context.Settings
                    .AsNoTracking()
                    .ToList();

                foreach (var item in items)
                {
                    Data.Add(item.ParamName, item.ParamValue);
                }
            }
        }
    }
}
