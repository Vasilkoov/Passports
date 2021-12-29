using FilesStorage.Settings.Interfaces;

namespace FilesStorage.Settings
{
    /// <summary>
    /// Дополнительные настройки приложения, которые поднимаются из appsettings.json
    /// </summary>
    internal class ExtraAppSettings : IExtraAppSettings
    {
        /// <summary>
        /// Путь zip архива, который будет скачиваться с сайте МВД
        /// </summary>
        public string ZipFilePath { get; set; }
        /// <summary>
        /// CRON выражение для ежедневного выражения
        /// </summary>
        public string CronExpForDailyUpdate { get; set; }
    }
}
