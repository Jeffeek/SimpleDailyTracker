using AutoMapper;
using SimpleDailyTracker.Application.Models;

namespace SimpleDailyTracker.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserImportModel, FullUserInformation>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User));

            CreateMap<FullUserInformation, UserExportModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}