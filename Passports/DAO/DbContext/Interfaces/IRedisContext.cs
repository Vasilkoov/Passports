using System.Threading.Tasks;

namespace FilesStorage.DAO.DbContext.Interfaces
{
    /// <summary>
    /// Интерфейс для работы с Redis в проекте
    /// </summary>
    internal interface IRedisContext
    {
        /// <summary>
        /// Получить список строк паспорта из хранища Redis
        /// </summary>
        Task<string[]> GetPassportLinesAsync(string serial, string number);

        /// <summary>
        /// Записать строки с датами паспорта в хранилище
        /// </summary>
        Task SetPassportInfoAsync(string serial, string number, string[] values);

    }
}
