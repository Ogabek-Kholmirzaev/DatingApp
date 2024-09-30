using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDto>()
            .ForMember(d => d.PhotoUrl, o => o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain)!.Url));

        CreateMap<Photo, PhotoDto>();
    }
}