namespace FilesStorage.Settings.Interfaces
{
    /// <summary>
    /// Интерфейс для хранения корня папки с файлами паспортов
    /// </summary>
    internal interface IFileSystemRootSettings
    {
        /// <summary>
        /// Сам путь к паспортам
        /// </summary>
        public string RootPath { get; }
    }
}
