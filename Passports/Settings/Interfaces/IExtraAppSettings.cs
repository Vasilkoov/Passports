namespace FilesStorage.Settings.Interfaces
{
    /// <summary>
    /// Интерфейс для хранения настроек из appsettings.json
    /// </summary>
    internal interface IExtraAppSettings
    {
        /// <summary>
        /// Путь архива для обновления
        /// </summary>
        public string ZipFilePath { get; }
        /// <summary>
        /// CRON выражение для ежедневного обновления
        /// </summary>
        public string CronExpForDailyUpdate { get; }
    }
}
