using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FilesStorage.DAO.Entities;

namespace FilesStorage.DAO.Repositories.Interfaces
{
    /// <summary>
    ///  Реализация репозитория для паспортов
    /// </summary>
    internal interface IPassportRepository
    {
        /// <summary>
        /// Проверить, есть ли активный паспорт в базе
        /// </summary>
        Task<bool> IsPassportExistsAsync(string serial, string number);
        /// <summary>
        /// Список паспортов, у которых были изменения за пришедшую дату
        /// </summary>
        Task<List<Passport>> GetPassportsByDateAsync(DateTime date);
        /// <summary>
        /// Получаем историю паспорта по серии и номеру
        /// </summary>
        Task<List<Passport>> GetPassportHistoryAsync(string serial, string number);

        /// <summary>
        /// Метод для обновления данных паспортов
        /// </summary>
        /// <param name="newCsvPath"> Физический путь нового CSV файла </param>
        Task UpdatePassportsWithNewFileAsync(string newCsvPath);
    }
}
