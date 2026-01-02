using System.Collections.Generic;
using System.Threading.Tasks;


namespace AI.OfferService.Domain.OfferFeature
{
    public interface IOfferRepository
    {
        Task<OfferEntity?> GetByIdAsync(long id);
        Task<IEnumerable<OfferEntity>> GetAllAsync();
        Task AddAsync(OfferEntity entity);
        Task UpdateAsync(OfferEntity entity);
        Task DeleteAsync(long id);
        Task SaveChangesAsync();
    }
}
