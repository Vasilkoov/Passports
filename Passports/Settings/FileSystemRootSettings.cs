using FilesStorage.Settings.Interfaces;

namespace FilesStorage.Settings
{
    /// <summary>
    /// Класс для хранения пути папки с паспортами
    /// </summary>
    internal class FileSystemRootSettings : IFileSystemRootSettings
    {
        /// <summary>
        /// Путь к паспортам
        /// </summary>
        public string RootPath { get; set; }
    }
}
