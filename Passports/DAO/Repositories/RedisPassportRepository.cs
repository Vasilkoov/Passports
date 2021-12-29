using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FilesStorage.DAO.DbContext.Interfaces;
using FilesStorage.DAO.Entities;
using FilesStorage.DAO.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace FilesStorage.DAO.Repositories
{
    /// <summary>
    /// Реализация репозитория паспортов при помощи Redis
    /// </summary>
    internal class RedisPassportRepository : IPassportRepository
    {
        private readonly ILogger<RedisPassportRepository> _logger;
        private readonly IRedisContext _redisContext;

        public RedisPassportRepository(ILogger<RedisPassportRepository> logger, IRedisContext redisContext)
        {
            _logger = logger;
            _redisContext = redisContext;
        }

        /// <summary>
        /// Проверяем, есть ли такой паспорт (не удалённый) в БД
        /// </summary>
        public async Task<bool> IsPassportExistsAsync(string serial, string number)
        {
            var lines = await _redisContext.GetPassportLinesAsync(serial, number).ConfigureAwait(false);
            var list = GetListFromLines(lines, serial, number);
            return list.Count > 0 && !list.Last().Deleted.HasValue;
        }

        /// <summary>
        /// Поиск паспортов по дате (не требуется для Redis)
        /// </summary>
        public Task<List<Passport>> GetPassportsByDateAsync(DateTime date)
        {
            // для поиска в redis этого не требуется
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить историю паспорта по серии и номеру
        /// </summary>
        public async Task<List<Passport>> GetPassportHistoryAsync(string serial, string number)
        {
            var result = await _redisContext.GetPassportLinesAsync(serial, number).ConfigureAwait(false);
            return GetListFromLines(result, serial, number);
        }


        /// <summary>
        /// Обновление БД при помощи нового CSV файла
        /// </summary>
        public async Task UpdatePassportsWithNewFileAsync(string newCsvPath)
        {
            try
            {
                using var sr = new StreamReader(newCsvPath);
                string line;
                while ((line = await sr.ReadLineAsync().ConfigureAwait(false)) != null)
                {
                    var fields = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (fields.Length != 2)
                    {
                        continue;
                    }

                    var serial = fields[0];
                    var number = fields[1];

                    var check = await _redisContext.GetPassportLinesAsync(fields[0], fields[1]).ConfigureAwait(false);
                    if (check.Length == 0)
                    {
                        await _redisContext.SetPassportInfoAsync(serial, number, new[] { $"Added#{DateTime.Now}" }).ConfigureAwait(false);
                    }
                    else if (check.Last().StartsWith("Deleted"))
                    {
                        var list = check.ToList();
                        list.Add($"Added#{DateTime.Now}");
                        await _redisContext.SetPassportInfoAsync(serial, number, list.ToArray()).ConfigureAwait(false);
                    }
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }


        private List<Passport> GetListFromLines(string[] redisLines, string serial, string number)
        {
            var result = new List<Passport>();
            var curPassport = new Passport(serial, number);

            //идём по всем строкам
            for (var i = 0; i < redisLines.Length; i += 2)
            {
                //пропускаем, сколько прошли и берём пару (Added и Deleted)
                var lines = redisLines.Skip(i).Take(2);
                foreach (var line in lines)
                {
                    var fields = line.Split('#');
                    if (fields.Length != 2)
                    {
                        continue;
                    }

                    if (!DateTime.TryParse(fields[1], out var dt))
                    {
                        continue;
                    }

                    if (line.StartsWith("Added"))
                    {
                        curPassport.Added = dt;
                    }
                    else
                    {
                        curPassport.Deleted = dt;
                    }
                }
                result.Add(curPassport);
                curPassport = new Passport(serial, number);
            }

            return result;
        }
    }
}
