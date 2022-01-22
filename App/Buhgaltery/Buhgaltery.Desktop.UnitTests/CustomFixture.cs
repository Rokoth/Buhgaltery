using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Buhgaltery.Common;
using Buhgaltery.DbClient;
using Buhgaltery.DbClient.Interface;
using Buhgaltery.DbClient.Model;
using Buhgaltery.DbClient.Repository;
using System.Data.SQLite;

namespace Buhgaltery.Desktop.UnitTests
{

    public class CustomFixture : IDisposable
    {
        public string ConnectionString { get; private set; }       
        public string DatabaseName { get; private set; }
        public ServiceProvider ServiceProvider { get; private set; }

        public CustomFixture()
        {

            Log.Logger = new LoggerConfiguration()
             .MinimumLevel.Verbose()
             .WriteTo.File(Path.Combine(Directory.GetCurrentDirectory(), "test-log.txt"))
             .CreateLogger();

            var serviceCollection = new ServiceCollection();
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();

            DatabaseName = $"buhgaltery_client_test_{DateTimeOffset.Now:yyyyMMdd_hhmmss}.db";
            ConnectionString = $"Data Source={DatabaseName};";            
            serviceCollection.Configure<CommonOptions>(config);
           
            serviceCollection.AddLogging(configure => configure.AddSerilog());
            serviceCollection.AddScoped<IErrorNotifyService, ErrorNotifyService>();

            serviceCollection.AddDbContext<DbSqLiteContext>(opt => opt.UseSqlite(ConnectionString));
            serviceCollection.AddScoped<IRepository<Settings>, Repository<Settings>>();
          
            serviceCollection.ConfigureAutoMapper();
            ServiceProvider = serviceCollection.BuildServiceProvider();

            ServiceProvider.GetRequiredService<IOptions<CommonOptions>>().Value.ConnectionStrings = new System.Collections.Generic.Dictionary<string, string>()
            {
                { "MainConnection", ConnectionString}
            };
            Deploy();
        }

        private void Deploy()
        {
            SQLiteConnection.CreateFile(DatabaseName);

            SQLiteConnection m_dbConnection = new SQLiteConnection(ConnectionString);
            m_dbConnection.Open();

            string sql = "PRAGMA soft_heap_limit=5; create table if not exists settings(id int not null primary key, param_name varchar(100) not null, param_value varchar(1000) not null); ";

            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();

            m_dbConnection.Close();
        }

        public void Dispose()
        {
            var logger = ServiceProvider.GetRequiredService<ILogger<CustomFixture>>();
            try
            {
                SQLiteConnection.ClearAllPools();
                if (File.Exists(DatabaseName))                {
                    
                    File.Delete(DatabaseName);
                }
            }
            catch(Exception ex)
            {
                logger.LogError($"Error on dispose: {ex.Message}; StackTrace: {ex.StackTrace}");
            }
        }
    }
}
