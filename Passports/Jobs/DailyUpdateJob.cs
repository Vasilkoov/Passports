using System;
using System.Threading;
using System.Threading.Tasks;
using FilesStorage.Updaters.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;
using RedLockNet.SERedis;

namespace FilesStorage.Jobs
{
    /// <summary>
    /// Сервис для ежедневных обновлений данных паспортов
    /// </summary>
    // Вот это можно выключить, если используем распределенные блокировки
    // [DisallowConcurrentExecution]
    internal class DailyUpdateJob : IJob
    {
        private readonly ILogger<DailyUpdateJob> _logger;
        private readonly IUpdater _updater;
        private readonly RedLockFactory _redLockFactory;

        public DailyUpdateJob(IUpdater updater, ILogger<DailyUpdateJob> logger, RedLockFactory redLockFactory)
        {
            _logger = logger;
            _updater = updater;
            _redLockFactory = redLockFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            TimeSpan expiry = TimeSpan.FromMinutes(40);
            await using var redLock = await _redLockFactory.CreateLockAsync(nameof(DailyUpdateJob), expiry);
            if (redLock.IsAcquired)
            {   
                Console.WriteLine($"{DateTime.Now:hh:mm:ss} - Джоба создала лок");
                //имитация бурной деятельности
                //джобы запускаются каждые 4 секунд, а лок живет 7 секунд
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
            else
            {
                Console.WriteLine($"{DateTime.Now:hh:mm:ss} - Джоба не запущена, т.к. стоит лок");
            }

            //try
            //{
            //    await _updater.UpdateAsync().ConfigureAwait(false);
            //    _logger.LogInformation($"{DateTime.Now} - Обновление прошло успешно");
            //}
            //catch (Exception e)
            //{
            //    _logger.LogInformation($"{DateTime.Now} - При обновлении произошла ошибка: " + e.Message);
            //}
        }
    }
}
