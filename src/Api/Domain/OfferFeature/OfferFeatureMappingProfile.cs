using AutoMapper;

namespace AI.OfferService.Domain.OfferFeature
{
    public class OfferFeatureMappingProfile : Profile
    {
        public OfferFeatureMappingProfile()
        {
            CreateMap<CreateOfferRequestDto, OfferEntity>()
                .ForMember(dest => dest.OfferId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<UpdateOfferRequestDto, OfferEntity>();
        }
    }
}
