using AI.OfferService.Domain.Entities;

namespace AI.OfferService.Domain.Repositories;

public interface IOfferRepository
{
    Task<Offer?> GetByIdAsync(long id);
    Task<List<Offer>> GetAllAsync(long? sellerId = null);
    Task<Offer> CreateAsync(Offer offer);
    Task<Offer?> UpdateAsync(Offer offer);
    Task<bool> DeleteAsync(long id);
}