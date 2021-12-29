using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FilesStorage.DAO.Entities;
using FilesStorage.DAO.Repositories.Interfaces;
using FilesStorage.FilesHandlers.Interfaces;
using FilesStorage.Settings.Interfaces;
using Microsoft.Extensions.Logging;

namespace FilesStorage.DAO.Repositories
{
    /// <summary>
    /// Реализация репозитория при помощи обычной файловой системы
    /// </summary>
    internal class FileSystemPassportRepository : IPassportRepository
    {
        private readonly string _rootPath;
        private readonly ILogger<FileSystemPassportRepository> _logger;
        private readonly IFileSystemManager _fileManager;

        public FileSystemPassportRepository(ILogger<FileSystemPassportRepository> logger, IFileSystemRootSettings rootSettings, IFileSystemManager fileManager)
        {
            _logger = logger;
            _rootPath = rootSettings.RootPath;
            _fileManager = fileManager;
        }

        /// <summary>
        /// Проверить, существует ли паспорт
        /// </summary>
        public async Task<bool> IsPassportExistsAsync(string serial, string number)
        {
            var path = GetPathBySerialAndNumber(serial, number);
            if (!_fileManager.IsDirectoryExists(path))
            {
                return false;
            }

            var passports = await GetListFromFileAsync(path, serial, number).ConfigureAwait(false);

            return passports.Count > 0 && !passports.Last().Deleted.HasValue;
        }

        /// <summary>
        /// Поиск паспортов по дате (не требуется для Redis)
        /// </summary>
        public Task<List<Passport>> GetPassportsByDateAsync(DateTime date)
        {
            //пока не реализовывать для этого репозитория
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получаем историю паспортов по серии и номеру
        /// </summary>
        public async Task<List<Passport>> GetPassportHistoryAsync(string serial, string number)
        {
            var path = GetPathBySerialAndNumber(serial, number);
            return !_fileManager.IsDirectoryExists(path) ? new List<Passport>() : await GetListFromFileAsync(path, serial, number).ConfigureAwait(false);
        }

        /// <summary>
        /// Обновляем наши паспорта при помощи новой csv
        /// </summary>
        public async Task UpdatePassportsWithNewFileAsync(string newCsvPath)
        {
            try
            {
                var accessDate = DateTime.Now;
                using var sr = _fileManager.ReadFileInStream(newCsvPath);
                string line;
                while ((line = await sr.ReadLineAsync().ConfigureAwait(false)) != null)
                {
                    var fields = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (fields.Length != 2)
                    {
                        continue;
                    }

                    var list = SplitPassportString(string.Concat(fields));
                    var path = Path.Combine(_rootPath, Path.Combine(list));

                    var filePath = Path.Combine(path, "Passport.txt");
                    //проверяем есть ли такой файл
                    if (!_fileManager.IsDirectoryExists(path))
                    {
                        //если нет, то добавляем его
                        _fileManager.CreateDirectory(path);
                        await _fileManager.WriteAllTextInFileAsync(filePath, $"Added#{DateTime.Now}").ConfigureAwait(false);
                    }
                    else
                    {
                        //если уже имеется, то проверим, был ли он удалён. Если удалён, то опять добавим
                        var lines = await _fileManager.ReadAllLinesFromFileAsync(filePath).ConfigureAwait(false);
                        if (lines.Last().StartsWith("Deleted"))
                        {
                            await _fileManager.AppendAllLinesInFileAsync(filePath, new[] { $"Added#{DateTime.Now}" }).ConfigureAwait(false);
                        }
                    }
                }

                //теперь ищем те, у которых последнеее время записи меньше чем accessDate => они были удалены, нужно пометить это
                var files = _fileManager.GetAllFilesFromChildDirectories(_rootPath, "*.txt")
                    .Where(f => _fileManager.GetLastAccessTime(f) < accessDate);

                foreach (var file in files)
                {
                    await _fileManager.AppendAllLinesInFileAsync(file, new[] { $"Deleted#{DateTime.Now}" }).ConfigureAwait(false);
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        private static string[] SplitPassportString(string str)
        {
            var list = new List<string>();
            var i = 0;
            while (i < str.Length - 1)
            {
                list.Add(str.Substring(i, 2));
                i += 2;
            }
            return list.ToArray();
        }

        private async Task<List<Passport>> GetListFromFileAsync(string path, string serial, string number)
        {
            var fileLines = await _fileManager.ReadAllLinesFromFileAsync(Path.Combine(path, "Passport.txt")).ConfigureAwait(false);

            var result = new List<Passport>();
            var curPassport = new Passport(serial, number);

            //идём по всем строкам
            for (var i = 0; i < fileLines.Length; i += 2)
            {
                //пропускаем, сколько прошли и берём пару (Added и Deleted)
                var lines = fileLines.Skip(i).Take(2);
                foreach (var line in lines)
                {
                    var fields = line.Split('#');
                    if (fields.Length != 2)
                    {
                        continue;
                    }

                    if (!DateTime.TryParse(fields[1], out var dt))
                    {
                        continue;
                    }

                    if (line.StartsWith("Added"))
                    {
                        curPassport.Added = dt;
                    }
                    else
                    {
                        curPassport.Deleted = dt;
                    }
                }
                result.Add(curPassport);
                curPassport = new Passport(serial, number);
            }

            return result;
        }

        /// <summary>
        /// Получаем путь к файлу при помощи серии и номера
        /// </summary>
        public string GetPathBySerialAndNumber(string serial, string number)
        {
            var pathList = SplitPassportString(string.Concat(serial, number));
            return Path.Combine(_rootPath, Path.Combine(pathList));
        }
    }
}
