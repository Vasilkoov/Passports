using System.Threading.Tasks;

namespace FilesStorage.FilesHandlers.Interfaces
{
    /// <summary>
    /// Интерфейс загрузки файла
    /// </summary>
    internal interface IDownloader
    {
        /// <summary>
        /// Основной метод загрузки архива
        /// </summary>
        Task DownloadZipFileAsync();
    }
}
