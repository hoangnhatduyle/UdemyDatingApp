using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.PhotoUrl, 
                           opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age,
                           opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            //1st parameter: which property we want to effect
            //2nd parameter: options, how to change that property
            CreateMap<Photo, PhotoDto>();
        }
    }
}