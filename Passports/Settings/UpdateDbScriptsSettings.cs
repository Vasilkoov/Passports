using FilesStorage.Settings.Interfaces;

namespace FilesStorage.Settings
{
    /// <summary>
    /// Класс контейнер для хранения скриптов обновления БД. Берётся из appsettings.json
    /// </summary>
    internal class UpdateDbScriptsSettings : IUpdateDbScriptsSettings
    {
        /// <summary>
        /// Скрипт для ипорта csv файла в базу
        /// </summary>
        public string ImportFileToDb { get; set; }
        /// <summary>
        /// Скрипты для обновлений БД из файла
        /// </summary>
        public string[] Scripts { get; set; }
    }
}
