using System.Collections.Generic;
using System.Threading.Tasks;

namespace AI.OfferService.Domain.OfferFeature
{
    public interface IVehicleRepository
    {
        Task<VehicleEntity?> GetByVinAsync(string vin);
        Task<VehicleEntity?> GetByIdAsync(int id);
        Task<IEnumerable<VehicleEntity>> GetAllAsync();
        Task AddAsync(VehicleEntity entity);
        Task UpdateAsync(VehicleEntity entity);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
        Task<VehicleEntity?> GetByDetailsAsync(int year, string make, string model, string trim, string vin);
    }
}
