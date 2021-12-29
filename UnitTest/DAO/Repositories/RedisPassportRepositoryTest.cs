using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using FilesStorage.DAO.DbContext.Interfaces;
using FilesStorage.DAO.Entities;
using FilesStorage.DAO.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace UnitTest.DAO.Repositories
{
    internal class RedisPassportRepositoryTest
    {
        [TestCase("1234", "123456", false)]
        [TestCase("1234", "567890", true)]
        public async Task IsPassportExistsTestAsync(string serial, string number, bool expectedResult)
        {
            var mockedLogger = new Mock<ILogger<RedisPassportRepository>>();
            var mockedRedisContext = new Mock<IRedisContext>();

            var repository = new RedisPassportRepository(mockedLogger.Object, mockedRedisContext.Object);

            mockedRedisContext.Setup(e => e.GetPassportLinesAsync(serial, number)).Returns(() =>
            {
                var list = new List<string> { $"Added#{DateTime.Now}" };
                if (!expectedResult)
                {
                    list.Add($"Deleted#{DateTime.Now}");
                }
                return Task.FromResult(list.ToArray());
            });

            var result = await repository.IsPassportExistsAsync(serial, number);

            mockedRedisContext.Verify(e => e.GetPassportLinesAsync(serial, number), Times.Once);

            Assert.AreEqual(result, expectedResult);
        }


        [Description("Тест получения истории паспорта при помощи файлового репозитория")]
        [TestCase(false, "1234", "123456")]
        [TestCase(true, "1234", "567890")]
        public async Task GetPassportHistoryTestAsync(bool isExists, string serial, string number)
        {
            var mockedLogger = new Mock<ILogger<RedisPassportRepository>>();
            var mockedRedisContext = new Mock<IRedisContext>();

            var repository = new RedisPassportRepository(mockedLogger.Object, mockedRedisContext.Object);
            var date = DateTime.Now.ToString(CultureInfo.InvariantCulture);

            mockedRedisContext.Setup(e => e.GetPassportLinesAsync(serial, number))
                .Returns(() => isExists ?
                    Task.FromResult(new[] { $"Added#{date}", $"Deleted#{date}" }) :
                    Task.FromResult(Array.Empty<string>()));

            var result = await repository.GetPassportHistoryAsync(serial, number);

            mockedRedisContext.Verify(e => e.GetPassportLinesAsync(serial, number), Times.Once);

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
