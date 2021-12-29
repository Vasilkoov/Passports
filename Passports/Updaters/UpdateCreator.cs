using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FilesStorage.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;

namespace FilesStorage.Updaters
{
    /// <summary>
    /// Статик класс для добавления Quartz
    /// </summary>
    internal static class UpdateCreator
    {
        /// <summary>
        /// Добавляем Quartz с CRON выражением из appsettings.json
        /// </summary>
        public static void AddQuartz(this IServiceCollection services, IConfiguration configuration, string cronExpression)
        {
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
                var jobKey = new JobKey(nameof(DailyUpdateJob));
                q.AddJob<DailyUpdateJob>(jobKey);
                q.AddTrigger(t => t
                    .WithIdentity(jobKey.Name + "Trigger")
                    .WithPriority(1)
                    .ForJob(jobKey)
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(4).RepeatForever()) // для примера сделал интервал в 4 секунды
                    //.WithCronSchedule(cronExpression)
                );
            });

            var redisConnection = configuration.GetConnectionString("RedisConnection");
            if (!string.IsNullOrEmpty(redisConnection))
            {
                var url = redisConnection.Split(',').FirstOrDefault();
                if (url.Split(':').Length == 2)
                {
                    var splitted = url.Split(':');
                    var host = splitted[0];
                    var port = Convert.ToInt32(splitted[1]);
                    var endPoints = new List<RedLockEndPoint>
                    {
                        new DnsEndPoint(host, port)
                        //можно добавлять и больше
                    };
                    var redlockFactory = RedLockFactory.Create(endPoints);
                    services.AddSingleton(redlockFactory);
                }
            }
            
            services.AddTransient<DailyUpdateJob>();
            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        }
    }

}
