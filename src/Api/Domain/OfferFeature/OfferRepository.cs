using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using AI.OfferService.Models;

namespace AI.OfferService.Domain.OfferFeature
{
    public class OfferRepository : IOfferRepository
    {
        private readonly AicodeChallengeDbContext _dbContext;
        public OfferRepository(AicodeChallengeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OfferEntity?> GetByIdAsync(long id)
        {
            var offer = await _dbContext.Offers.FindAsync(id);
            return offer != null ? MapToOfferEntity(offer) : null;
        }

        public async Task<IEnumerable<OfferEntity>> GetAllAsync()
        {
            var offers = await _dbContext.Offers.ToListAsync();
            return offers.Select(MapToOfferEntity);
        }

        public async Task AddAsync(OfferEntity entity)
        {
            var offer = MapToOffer(entity);
            await _dbContext.Offers.AddAsync(offer);
        }

        public Task UpdateAsync(OfferEntity entity)
        {
            var offer = MapToOffer(entity);
            offer.OfferId = entity.OfferId;
            _dbContext.Offers.Update(offer);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _dbContext.Offers.FindAsync(id);
            if (entity != null)
                _dbContext.Offers.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        private static OfferEntity MapToOfferEntity(Offer offer)
        {
            return new OfferEntity
            {
                OfferId = offer.OfferId,
                SellerId = offer.SellerId,
                VIN = offer.VIN,
                OfferAmount = offer.OfferAmount ?? 0,
                City = offer.City,
                State = offer.State,
                Country = offer.Country,
                Status = offer.Status,
                CreatedAt = offer.CreatedAt,
                UpdatedAt = offer.UpdatedAt,
                VehicleId = offer.VehicleId
            };
        }

        private static Offer MapToOffer(OfferEntity entity)
        {
            return new Offer
            {
                OfferId = entity.OfferId,
                SellerId = entity.SellerId,
                VIN = entity.VIN,
                OfferAmount = entity.OfferAmount,
                City = entity.City,
                State = entity.State,
                Country = entity.Country,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                VehicleId = (int)entity.VehicleId
            };
        }
    }
}

