using AI.OfferService.Domain.Entities;

namespace AI.OfferService.Domain.Repositories;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(long id);
    Task<Vehicle?> GetByVINAsync(string vin);
    Task<Vehicle> CreateAsync(Vehicle vehicle);
}