using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using FilesStorage.DAO.Repositories.Interfaces;
using FilesStorage.DAO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using FilesStorage.Settings.Interfaces;

namespace FilesStorage.Controllers
{
    /// <summary> Контроллер для получения информация об неактивности паспорта </summary>
    [ApiController]
    [Route("[controller]")]
    public class PassportsController : ControllerBase
    {
        private readonly string _path;
        private readonly IPassportRepository _passportRepository;
        private readonly IMapper _mapper;
        private static string ErrorMessage => "Длина серии паспорта должна составлять 4 символа, а длина номера - 6 символов";

        public PassportsController(IServiceProvider serviceProvider, IMapper mapper)
        {
            _path = serviceProvider.GetService<IExtraAppSettings>()?.ZipFilePath;
            _passportRepository = serviceProvider.GetService<IPassportRepository>();
            _mapper = mapper;
        }

        [Route("Download")]
        [HttpGet]
        public IActionResult Download()
        {
            if (!System.IO.File.Exists(_path))
            {
                return NotFound();
            }

            Stream stream = System.IO.File.OpenRead(_path);
            return File(stream, "application/octet-stream", "data.zip");
        }


        [Route("CheckIfPassportExists")]
        [HttpGet]
        public async Task<IActionResult> CheckIfPassportExistsAsync(string serial, string number)
        {
            if (serial.Length != 4 && number.Length != 6)
            {
                return new BadRequestObjectResult(ErrorMessage);
            }

            var isExists = await _passportRepository.IsPassportExistsAsync(serial, number).ConfigureAwait(false);
            return new ObjectResult(isExists);
        }

        [Route("GetAllPassportsByDate")]
        [HttpGet]
        public async Task<IActionResult> GetAllPassportsByDateAsync(DateTime date)
        {
            var dateHistory = await _passportRepository.GetPassportsByDateAsync(date).ConfigureAwait(false);
            var model = _mapper.Map<List<PassportViewModel>>(dateHistory);
            return new ObjectResult(model);
        }

        [Route("CheckPassportHistory")]
        [HttpGet]
        public async Task<IActionResult> CheckPassportHistoryAsync(string serial, string number)
        {
            if (serial.Length != 4 && number.Length != 6)
            {
                return new BadRequestObjectResult(ErrorMessage);
            }

            var passportHistory = await _passportRepository.GetPassportHistoryAsync(serial, number).ConfigureAwait(false);
            var model = _mapper.Map<List<PassportViewModel>>(passportHistory);

            return new ObjectResult(model);
        }
    }
}
