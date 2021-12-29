using System;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using FilesStorage.DAO.Entities;

namespace FilesStorage.DAO.ViewModels
{
    /// <summary>
    /// Класс с моделью паспорта для АвтоМапера
    /// </summary>
    [AutoMap(typeof(Passport))]
    internal class PassportViewModel
    {
        [SourceMember(nameof(Passport.Serial))]
        public string PassportSerial { get; set; }

        [SourceMember(nameof(Passport.Number))]
        public string PassportNumber { get; set; }
        
        public DateTime? Added { get; set; }
        
        public DateTime? Deleted { get; set; }
    }
}

