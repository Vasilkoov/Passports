using System;
using System.IO;
using System.Threading.Tasks;
using FilesStorage.FilesHandlers.Interfaces;

namespace FilesStorage.FilesHandlers
{
    /// <summary>
    /// Реализация интерфейса для переопределния работы с файлами
    /// </summary>
    internal class FileSystemManager : IFileSystemManager
    {
        /// <summary>
        /// Создаёт папку
        /// </summary>
        public void CreateDirectory(string dirPath) => Directory.CreateDirectory(dirPath);

        /// <summary>
        /// Проверяет, существует ли папка
        /// </summary>
        public bool IsDirectoryExists(string dirPath) => Directory.Exists(dirPath);

        /// <summary>
        /// Записывает строку в файл
        /// </summary>
        public async Task WriteAllTextInFileAsync(string filePath, string content) => await File.WriteAllTextAsync(filePath, content).ConfigureAwait(false);

        /// <summary>
        /// Записывает строки в файл
        /// </summary>
        public async Task AppendAllLinesInFileAsync(string filePath, string[] lines) => await File.AppendAllLinesAsync(filePath, lines).ConfigureAwait(false);

        /// <summary>
        /// Получить список всех файлов по маске из дочерних каталогов
        /// </summary>
        public string[] GetAllFilesFromChildDirectories(string directoryPath, string fileMask) =>
            Directory.GetFiles(directoryPath, fileMask, SearchOption.AllDirectories);

        /// <summary>
        /// Прочитать файл в поток
        /// </summary>
        public StreamReader ReadFileInStream(string filePath) => new(filePath);

        /// <summary>
        /// Прочтитать все строки из файла
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<string[]> ReadAllLinesFromFileAsync(string filePath) => await File.ReadAllLinesAsync(filePath).ConfigureAwait(false);

        /// <summary>
        /// Получить время, когда последний раз обращались по данному пути
        /// </summary>
        public DateTime GetLastAccessTime(string path) => File.GetLastAccessTime(path);
    }
}
