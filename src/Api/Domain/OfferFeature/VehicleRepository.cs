using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using AI.OfferService.Models;

namespace AI.OfferService.Domain.OfferFeature
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly AicodeChallengeDbContext _dbContext;
        public VehicleRepository(AicodeChallengeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<VehicleEntity?> GetByVinAsync(string vin)
        {
            var vehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.VIN == vin);
            return vehicle != null ? MapToVehicleEntity(vehicle) : null;
        }

        public async Task<VehicleEntity?> GetByIdAsync(int id)
        {
            var vehicle = await _dbContext.Vehicles.FindAsync((long)id);
            return vehicle != null ? MapToVehicleEntity(vehicle) : null;
        }

        public async Task<IEnumerable<VehicleEntity>> GetAllAsync()
        {
            var vehicles = await _dbContext.Vehicles.ToListAsync();
            return vehicles.Select(MapToVehicleEntity);
        }

        public async Task AddAsync(VehicleEntity entity)
        {
            var vehicle = MapToVehicle(entity);
            await _dbContext.Vehicles.AddAsync(vehicle);
        }

        public Task UpdateAsync(VehicleEntity entity)
        {
            var vehicle = MapToVehicle(entity);
            vehicle.Id = entity.Id;
            _dbContext.Vehicles.Update(vehicle);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbContext.Vehicles.FindAsync((long)id);
            if (entity != null)
                _dbContext.Vehicles.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<VehicleEntity?> GetByDetailsAsync(int year, string make, string model, string trim, string vin)
        {
            var vehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(v =>
                v.Year == year &&
                v.Make == make &&
                v.Model == model &&
                v.Trim == trim &&
                v.VIN == vin);
            return vehicle != null ? MapToVehicleEntity(vehicle) : null;
        }

        private static VehicleEntity MapToVehicleEntity(Vehicle vehicle)
        {
            return new VehicleEntity
            {
                Id = vehicle.Id,
                Make = vehicle.Make ?? string.Empty,
                Model = vehicle.Model ?? string.Empty,
                Year = vehicle.Year,
                Trim = vehicle.Trim ?? string.Empty,
                VIN = vehicle.VIN,
                CreatedAt = vehicle.CreatedAt,
                LastModifiedAt = vehicle.LastModifiedAt
            };
        }

        private static Vehicle MapToVehicle(VehicleEntity entity)
        {
            return new Vehicle
            {
                Id = entity.Id,
                Make = entity.Make,
                Model = entity.Model,
                Year = entity.Year,
                Trim = entity.Trim,
                VIN = entity.VIN,
                CreatedAt = entity.CreatedAt,
                LastModifiedAt = entity.LastModifiedAt
            };
        }
    }
}

