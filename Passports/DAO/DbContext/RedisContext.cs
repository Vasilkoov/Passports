using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FilesStorage.DAO.DbContext.Interfaces;
using StackExchange.Redis;

namespace FilesStorage.DAO.DbContext
{
    /// <summary>
    /// Реализация Redis контекста
    /// </summary>
    internal class RedisContext : IRedisContext
    {
        private readonly IDatabase _db;
        private readonly RedisKey _key;

        public RedisContext(IDatabase db)
        {
            _db = db;
            _key = new RedisKey("Passports");
        }

        /// <summary>
        /// Получаем строки с историей паспорта из Redis
        /// </summary>
        public async Task<string[]> GetPassportLinesAsync(string serial, string number)
        {
            var result = await _db.HashGetAsync(_key, new RedisValue(serial + number)).ConfigureAwait(false);

            return !result.HasValue ? Array.Empty<string>() : result.ToString().Split(Environment.NewLine);
        }

        /// <summary>
        /// Записать информацию о паспорте в Redis
        /// </summary>
        public async Task SetPassportInfoAsync(string serial, string number, string[] value)
        {
            var redisValue = new RedisValue(string.Join(Environment.NewLine, value));
            var fieldKey = new RedisValue(string.Concat(serial, number));
            await _db.HashSetAsync(_key, fieldKey, new RedisValue(redisValue)).ConfigureAwait(false);
        }
    }
}
