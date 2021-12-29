using System.Threading.Tasks;

namespace FilesStorage.FilesHandlers.Interfaces
{
    /// <summary>
    /// Интерфейс распаковки файла
    /// </summary>
    internal interface IExtractor
    {
        /// <summary>
        /// Основной метод распаковки файлов
        /// </summary>
        /// <returns>Возвращает физический путь к файлу</returns>
        Task<string> ExtractFileAsync();
    }
}
