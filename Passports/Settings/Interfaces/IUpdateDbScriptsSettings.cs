namespace FilesStorage.Settings.Interfaces
{
    /// <summary>
    /// Интерфейс для скриптов обновления БД
    /// </summary>
    internal interface IUpdateDbScriptsSettings
    {
        /// <summary>
        /// Скрипт для ипорта csv файла в базу
        /// </summary>
        public string ImportFileToDb { get; }
        /// <summary>
        /// Скрипты для обновлений БД из файла (хранится в appsettings.json)
        /// </summary>
        public string[] Scripts { get; }
    }
}
