namespace AI.OfferService.Domain.OfferFeature
{
    public interface IOfferService
    {
        Task<long> CreateOfferAsync(CreateOfferRequestDto request);
        Task UpdateOfferAsync(UpdateOfferRequestDto request);
    }
}
