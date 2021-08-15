using AutoMapper;
using SimpleDailyTracker.Application.Models;
using SimpleDailyTracker.UI.Models;

namespace SimpleDailyTracker.UI.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<FullUserInformation, UserViewModel>()
                .ReverseMap();

            CreateMap<FullUserInformation, UserExportModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}