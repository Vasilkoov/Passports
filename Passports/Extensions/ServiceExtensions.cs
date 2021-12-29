using System;
using FilesStorage.DAO.DbContext;
using FilesStorage.DAO.DbContext.Interfaces;
using FilesStorage.DAO.Repositories;
using FilesStorage.DAO.Repositories.Interfaces;
using FilesStorage.FilesHandlers;
using FilesStorage.FilesHandlers.Interfaces;
using FilesStorage.Settings;
using FilesStorage.Settings.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FilesStorage.Extensions
{
    /// <summary>
    /// Статический класс для расширений интерфейса IServiceCollection
    /// </summary>
    internal static class ServiceExtensions
    {
        /// <summary>
        /// Добавляем репозиторий в зависимости от того, что прописано в appsettings.json
        /// </summary>
        public static void AddRepository(this IServiceCollection services, IConfiguration conf)
        {   
            switch (conf.GetValue<string>("DbType").ToLower())
            {
                case "redis":
                    var db = ConnectionMultiplexer.Connect(conf.GetConnectionString("RedisConnection")).GetDatabase();
                    services.AddSingleton(db);
                    services.AddTransient<IRedisContext, RedisContext>();
                    services.AddTransient<IPassportRepository, RedisPassportRepository>();
                    break;
                case "pgsql":

                    //добавляем DbContext
                    services.AddDbContext<SqlContext>(options => options.UseNpgsql(conf.GetConnectionString("PgConnection")));
                    services.AddTransient<IPassportRepository, SqlPassportRepository>();

                    //читаем скрипты для ежедневного обновления
                    var updScripts = new UpdateDbScriptsSettings();
                    conf.Bind("UpdateDbScripts", updScripts);
                    services.AddSingleton<IUpdateDbScriptsSettings>(updScripts);
                    break;
                case "files":
                    var rootPath = new FileSystemRootSettings();
                    conf.Bind("FileSystemRoot", rootPath);
                    services.AddSingleton<IFileSystemRootSettings>(rootPath);
                    services.AddTransient<IPassportRepository, FileSystemPassportRepository>();
                    services.AddSingleton<IFileSystemManager, FileSystemManager>();
                    break;
                default:
                    throw new NotImplementedException("Неизвестный DbType. Проверьте файл appsettings.json. Секция DbType");
            }
        }
    }
}
