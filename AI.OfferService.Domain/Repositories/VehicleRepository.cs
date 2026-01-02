using AI.OfferService.Domain.Data;
using AI.OfferService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AI.OfferService.Domain.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly OfferDbContext _context;

    public VehicleRepository(OfferDbContext context)
    {
        _context = context;
    }

    public async Task<Vehicle?> GetByIdAsync(long id)
    {
        return await _context.Vehicles.FindAsync(id);
    }

    public async Task<Vehicle?> GetByVINAsync(string vin)
    {
        return await _context.Vehicles.FirstOrDefaultAsync(v => v.VIN == vin);
    }

    public async Task<Vehicle> CreateAsync(Vehicle vehicle)
    {
        vehicle.CreatedAt = DateTime.UtcNow;
        vehicle.LastModifiedAt = DateTime.UtcNow;
        
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();
        
        return vehicle;
    }
}