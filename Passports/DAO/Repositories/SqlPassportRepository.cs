using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesStorage.DAO.DbContext;
using FilesStorage.DAO.Entities;
using FilesStorage.DAO.Repositories.Interfaces;
using FilesStorage.Settings.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FilesStorage.DAO.Repositories
{
    /// <summary>
    /// Реализация паттерна репозиторий для паспортов через EntityFramework
    /// </summary>
    internal class SqlPassportRepository : IPassportRepository
    {
        private readonly ILogger<SqlPassportRepository> _logger;
        private readonly SqlContext _db;
        private readonly IUpdateDbScriptsSettings _updateSqlScripts;

        public SqlPassportRepository(SqlContext db, ILogger<SqlPassportRepository> logger, IUpdateDbScriptsSettings scripts)
        {
            _db = db;
            _updateSqlScripts = scripts;
            _logger = logger;
        }

        private IQueryable<Passport> GetBySerialAndNumber(string serial, string number) => _db.Passports.Where(p => p.Serial == serial && p.Number == number);

        /// <summary>
        /// Проверяет, есть ли не удалённый паспорт с такой серией и номером в базе
        /// </summary>
        public async Task<bool> IsPassportExistsAsync(string serial, string number)
        {
            return await GetBySerialAndNumber(serial, number).AnyAsync(pass => pass.Deleted == null).ConfigureAwait(false);
        }

        /// <summary>
        /// Получает список паспортов, которые были добавлены или удалены за определенную дату
        /// </summary>
        public async Task<List<Passport>> GetPassportsByDateAsync(DateTime date)
        {
            return await _db.Passports.Where(pass => pass.Added == date || pass.Deleted == date).ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Получает всю историю паспортов по серии и номеру
        /// </summary>
        public async Task<List<Passport>> GetPassportHistoryAsync(string serial, string number)
        {
            return await GetBySerialAndNumber(serial, number).ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Ежедневное обновление
        /// </summary>
        public async Task UpdatePassportsWithNewFileAsync(string newCsvPath)
        {
            //в appsettings.json для ImportFileToDb должно быть ключевое слово ###path### для того, чтобы подменить путь csv файла
            var commands = new List<string> { _updateSqlScripts.ImportFileToDb.Replace("###path###", newCsvPath) };
            commands.AddRange(_updateSqlScripts.Scripts);
            foreach (var command in commands)
            {
                await ExecSqlCommandAsync(command).ConfigureAwait(false);
            }
        }

        private async Task ExecSqlCommandAsync(string sql)
        {
            try
            {
                _db.Database.SetCommandTimeout(3600);
                await _db.Database.ExecuteSqlRawAsync(sql).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}
