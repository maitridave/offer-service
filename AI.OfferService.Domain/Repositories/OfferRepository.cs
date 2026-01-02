using AI.OfferService.Domain.Data;
using AI.OfferService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AI.OfferService.Domain.Repositories;

public class OfferRepository : IOfferRepository
{
    private readonly OfferDbContext _context;

    public OfferRepository(OfferDbContext context)
    {
        _context = context;
    }

    public async Task<Offer?> GetByIdAsync(long id)
    {
        return await _context.Offers
            .Include(o => o.Vehicle)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<List<Offer>> GetAllAsync(long? sellerId = null)
    {
        var query = _context.Offers.Include(o => o.Vehicle).AsQueryable();
        
        if (sellerId.HasValue)
        {
            query = query.Where(o => o.SellerId == sellerId.Value);
        }
        
        return await query.ToListAsync();
    }

    public async Task<Offer> CreateAsync(Offer offer)
    {
        offer.CreatedAt = DateTime.UtcNow;
        offer.LastModifiedAt = DateTime.UtcNow;
        
        _context.Offers.Add(offer);
        await _context.SaveChangesAsync();
        
        return await GetByIdAsync(offer.Id) ?? offer;
    }

    public async Task<Offer?> UpdateAsync(Offer offer)
    {
        var existing = await _context.Offers.FindAsync(offer.Id);
        if (existing == null) return null;

        existing.BuyerId = offer.BuyerId;
        existing.CarrierId = offer.CarrierId;
        existing.OfferAmount = offer.OfferAmount;
        existing.City = offer.City;
        existing.State = offer.State;
        existing.Country = offer.Country;
        existing.Status = offer.Status;
        existing.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetByIdAsync(offer.Id);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var offer = await _context.Offers.FindAsync(id);
        if (offer == null) return false;

        _context.Offers.Remove(offer);
        await _context.SaveChangesAsync();
        return true;
    }
}