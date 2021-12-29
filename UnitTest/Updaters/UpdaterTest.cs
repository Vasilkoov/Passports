using System.Threading.Tasks;
using FilesStorage.DAO.Repositories.Interfaces;
using FilesStorage.FilesHandlers.Interfaces;
using FilesStorage.Settings.Interfaces;
using FilesStorage.Updaters;
using Moq;
using NUnit.Framework;

namespace UnitTest.Updaters
{
    public class UpdaterTest
    {
        [Description("Тест обновления БД")]
        [TestCase(true, "data.csv", true)]
        [TestCase(false, "", false)]
        public async Task MainUpdaterTestAsync(bool downloadResult, string extractResult, bool expectedResult)
        {
            //создаем мок объекты
            var mockedRepository = new Mock<IPassportRepository>(MockBehavior.Default);
            var mockedFileProcessor = new Mock<IFileProcessor>(MockBehavior.Default);
            var mockedUpdateDbScripts = new Mock<IUpdateDbScriptsSettings>(MockBehavior.Default);
            mockedUpdateDbScripts.SetupGet(e => e.ImportFileToDb).Returns("str");

            //Делаем установки, что наши методы должны вернуть
            mockedFileProcessor.Setup(e => e.DownloadZipFileAsync()).Returns(Task.FromResult(downloadResult));
            mockedFileProcessor.Setup(e => e.ExtractFileAsync()).Returns(Task.FromResult(extractResult));
            mockedRepository.Setup(e => e.UpdatePassportsWithNewFileAsync(extractResult)).Returns(Task.FromResult(expectedResult));
            
            //создаем экземпляр
            var updater = new Updater(mockedRepository.Object, mockedFileProcessor.Object);

            //вызываем, что хотим проверить
            await updater.UpdateAsync();

            //проверяем, всё ли выполнялось столько раз, сколько нам нужно
            mockedFileProcessor.Verify(e => e.DownloadZipFileAsync(), Times.Once);
            mockedFileProcessor.Verify(e => e.ExtractFileAsync(), Times.Once);
        }
    }
}