using AutoMapper;
using FilesStorage.DAO.Entities;
using FilesStorage.DAO.ViewModels;

namespace FilesStorage.DAO.Profiles
{
    /// <summary>
    /// Профиль паспортов для АвтоМапера 
    /// </summary>
    internal class PassportProfile : Profile
    {
        public PassportProfile()
        {
            CreateMap<Passport, PassportViewModel>();
        }
    }
}
