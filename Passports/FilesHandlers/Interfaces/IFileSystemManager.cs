using System;
using System.IO;
using System.Threading.Tasks;

namespace FilesStorage.FilesHandlers.Interfaces
{
    /// <summary>
    /// Интерфейс для переопределения работы с файлами
    /// </summary>
    internal interface IFileSystemManager
    {
        /// <summary>
        /// Создать папку
        /// </summary>
        void CreateDirectory(string path);

        /// <summary>
        /// Проверяет, существует ли папка
        /// </summary>
        bool IsDirectoryExists(string path);

        /// <summary>
        /// Записать строку в файл
        /// </summary>
        Task WriteAllTextInFileAsync(string filePath, string content);

        /// <summary>
        /// Записать строки в файл
        /// </summary>
        Task AppendAllLinesInFileAsync(string filePath, string[] lines);

        /// <summary>
        /// Получить список всех файлов по маске из дочерних каталогов
        /// </summary>
        string[] GetAllFilesFromChildDirectories(string directoryPath, string fileMask);

        /// <summary>
        /// Прочитать файл в поток
        /// </summary>
        StreamReader ReadFileInStream(string filePath);

        /// <summary>
        /// Прочитать все строки из файла
        /// </summary>
        Task<string[]> ReadAllLinesFromFileAsync(string filePath);

        /// <summary>
        /// Получить время, когда последний раз обращались по данному пути
        /// </summary>
        DateTime GetLastAccessTime(string path);


    }
}
