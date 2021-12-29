using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using FilesStorage.DAO.Entities;
using FilesStorage.DAO.Repositories;
using FilesStorage.FilesHandlers.Interfaces;
using FilesStorage.Settings.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace UnitTest.DAO.Repositories
{
    internal class FileSystemPassportRepositoryTest
    {
        [Description("Тест проверки активности паспорта при помощи файлового репозитория")]
        [TestCase("1234", "123456", false)]
        [TestCase("1234", "567890", true)]
        public async Task IsPassportExistsTestAsync(string serial, string number, bool expectedResult)
        {
            var mockedLogger = new Mock<ILogger<FileSystemPassportRepository>>();
            var mockedRootSettings = new Mock<IFileSystemRootSettings>();
            var mockerFileManager = new Mock<IFileSystemManager>();

            mockedRootSettings.SetupGet(e => e.RootPath).Returns("D:\\Passports");

            var fileRepository = new FileSystemPassportRepository(mockedLogger.Object, mockedRootSettings.Object, mockerFileManager.Object);

            var path = fileRepository.GetPathBySerialAndNumber(serial, number);
            var filePath = Path.Combine(path, "Passport.txt");


            mockerFileManager.Setup(e => e.IsDirectoryExists(path)).Returns(true);

            mockerFileManager.Setup(e => e.ReadAllLinesFromFileAsync(filePath))
                .Returns(() =>
                {
                    var list = new List<string> { $"Added#{DateTime.Now}" };
                    if (!expectedResult)
                    {
                        list.Add($"Deleted#{DateTime.Now}");
                    }
                    return Task.FromResult(list.ToArray());
                });

            var result = await fileRepository.IsPassportExistsAsync(serial, number);

            mockerFileManager.Verify(e => e.IsDirectoryExists(path), Times.Once);
            mockerFileManager.Verify(e => e.ReadAllLinesFromFileAsync(filePath), Times.Once);


            Assert.AreEqual(result, expectedResult);
        }

        [Description("Тест получения истории паспорта при помощи файлового репозитория")]
        [TestCase(false, "1234", "123456")]
        [TestCase(true, "1234", "567890")]
        public async Task GetPassportHistoryTestAsync(bool isExists, string serial, string number)
        {
            var mockedLogger = new Mock<ILogger<FileSystemPassportRepository>>();
            var mockedRootSettings = new Mock<IFileSystemRootSettings>();
            var mockerFileManager = new Mock<IFileSystemManager>();

            mockedRootSettings.SetupGet(e => e.RootPath).Returns("D:\\Projects");

            var fileRepository = new FileSystemPassportRepository(mockedLogger.Object, mockedRootSettings.Object, mockerFileManager.Object);
            var path = fileRepository.GetPathBySerialAndNumber(serial, number);
            var filePath = Path.Combine(path, "Passport.txt");
            var date = DateTime.Now.ToString(CultureInfo.InvariantCulture);

            mockerFileManager.Setup(e => e.IsDirectoryExists(path)).Returns(isExists);

            mockerFileManager.Setup(e => e.ReadAllLinesFromFileAsync(filePath))
                .Returns(() => isExists ?
                    Task.FromResult(new[] { $"Added#{date}", $"Deleted#{date}" }) :
                    Task.FromResult(Array.Empty<string>()));

            var result = await fileRepository.GetPassportHistoryAsync(serial, number);


            mockerFileManager.Verify(e => e.IsDirectoryExists(path), Times.Once);
            mockerFileManager.Verify(e => e.ReadAllLinesFromFileAsync(filePath), isExists ? Times.Once : Times.Never);


            var expectedPassport = new Passport { Serial = serial, Number = number, Added = DateTime.Parse(date), Deleted = DateTime.Parse(date) };
            if (isExists && result.Count > 0)
            {
                var expectedSerialize = JsonConvert.SerializeObject(expectedPassport);
                var resultSerialize = JsonConvert.SerializeObject(result[0]);
                Assert.AreEqual(resultSerialize, expectedSerialize);
            }
            else
            {
                Assert.IsTrue(result.Count == 0);
            }
        }
    }
}
