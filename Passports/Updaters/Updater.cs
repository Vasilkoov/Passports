using System.Threading.Tasks;
using FilesStorage.DAO.Repositories.Interfaces;
using FilesStorage.FilesHandlers.Interfaces;
using FilesStorage.Updaters.Interfaces;

namespace FilesStorage.Updaters
{
    /// <summary> Статический класс для ежедневного обновления </summary>
    internal class Updater : IUpdater
    {
        private readonly IFileProcessor _fileProcess;
        private readonly IPassportRepository _passportRepository;

        public Updater(IPassportRepository passportRepository, IFileProcessor fileProcess)
        {
            _passportRepository = passportRepository;
            _fileProcess = fileProcess;
        }

        /// <summary>
        /// Основной метод обновления
        /// </summary>
        public async Task UpdateAsync()
        {
            await _fileProcess.DownloadZipFileAsync().ConfigureAwait(false);
            var csvPath = await _fileProcess.ExtractFileAsync().ConfigureAwait(false);
            await _passportRepository.UpdatePassportsWithNewFileAsync(csvPath).ConfigureAwait(false);
        }
    }
}
