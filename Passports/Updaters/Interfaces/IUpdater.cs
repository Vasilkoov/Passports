using System.Threading.Tasks;

namespace FilesStorage.Updaters.Interfaces
{
    /// <summary>
    /// Интерфейс для обновления данных паспортов
    /// </summary>
    internal interface IUpdater
    {
        /// <summary>
        /// Основной метод обновления
        /// </summary>
        Task UpdateAsync();
    }
}
