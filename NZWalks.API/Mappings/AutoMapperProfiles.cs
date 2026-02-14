using AutoMapper;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Region, RegionDto>().ReverseMap();
            CreateMap<Region, AddRegionDto>().ReverseMap();
            CreateMap<UpdateRegionDto, Region>().ReverseMap();

            CreateMap<AddWalkDto,Walk>().ReverseMap();
            CreateMap<UpdateWalkDto,Walk>().ReverseMap();
            CreateMap<Walk,WalkDto>().ReverseMap();
            
            CreateMap<Difficulty,DifficultyDto>().ReverseMap();

            //// For Partial Update - only map non-null properties
            ///CreateMap<UpdateRegionDto, Region>()
            ///    .ForAllMembers(opt => 
            ///         opt.Condition((src, dest, srcMember) => srcMember != null));
            
            /// For Partial Update - only map non-null properties
            ///CreateMap<UpdateWalkDto, Walk>()
            ///    .ForAllMembers(opt =>
            ///        opt.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}
