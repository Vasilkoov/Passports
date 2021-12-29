using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using FilesStorage.FilesHandlers.Interfaces;
using FilesStorage.Settings.Interfaces;
using Microsoft.Extensions.Logging;

namespace FilesStorage.FilesHandlers
{
    /// <summary>
    /// Класс для работы с файлом обновления
    /// </summary>
    internal class FileProcessor : IFileProcessor
    {
        private readonly string _path;
        private readonly ILogger<FileProcessor> _logger;

        public FileProcessor(ILogger<FileProcessor> logger, IExtraAppSettings sett)
        {
            _path = sett.ZipFilePath;
            _logger = logger;
        }
        /// <summary>
        /// Основной метод загрузки
        /// </summary>
        /// <returns></returns>
        public Task DownloadZipFileAsync()
        {
            //todo: скачиваем по url файл
            return Task.CompletedTask;
        }
        /// <summary>
        /// Основной метод распаковки
        /// </summary>
        public async Task<string> ExtractFileAsync()
        {
            var dir = Path.Combine(Path.GetDirectoryName(_path), "unzip");
            try
            {
                await Task.Run(() => ZipFile.ExtractToDirectory(_path, dir, true)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Ошибка при распаковке файла: {e.Message}");
                return null;
            }
            var files = Directory.GetFiles(dir);

            if (files.Length == 0)
            {
                _logger.LogError("Не найден необходимый csv файл");
                return null;
            }

            if (files.Length > 1)
            {
                _logger.LogError("В архиве содержится более одного файла");
                return null;
            }

            var csvPath = files[0];

            return Path.GetFullPath(csvPath);
        }
    }
}
