using System;
using System.Threading.Tasks;
using AI.OfferService.Shared;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace AI.OfferService.Domain.OfferFeature
{
    public class OfferService : IOfferService
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OfferService> _logger;
        private readonly IEventPublisher _publisher; // Assuming you have an event publisher interface

        public OfferService(IOfferRepository offerRepository, IVehicleRepository vehicleRepository, IMapper mapper, ILogger<OfferService> logger, IEventPublisher publisher)
        {
            _offerRepository = offerRepository;
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
            _logger = logger;
            _publisher = publisher;
        }

        public async Task<long> CreateOfferAsync(CreateOfferRequestDto request)
        {
            _logger.LogInformation("Creating offer for VIN {VIN}", request.VIN);
            var vehicle = await _vehicleRepository.GetByDetailsAsync(request.Year, request.Make, request.Model, request.Trim, request.VIN);
            if (vehicle == null)
            {
                vehicle = new VehicleEntity {
                    Year = request.Year,
                    Make = request.Make,
                    Model = request.Model,
                    Trim = request.Trim,
                    VIN = request.VIN,
                    CreatedAt = DateTime.UtcNow,
                    LastModifiedAt = DateTime.UtcNow
                };
                await _vehicleRepository.AddAsync(vehicle);
                await _vehicleRepository.SaveChangesAsync();
                _logger.LogInformation("Created new vehicle for VIN {VIN}", request.VIN);
            }
            var offer = _mapper.Map<OfferEntity>(request);
            offer.VehicleId = vehicle.Id;
            offer.CreatedAt = DateTime.UtcNow;
            offer.UpdatedAt = DateTime.UtcNow;
            await _offerRepository.AddAsync(offer);
            await _offerRepository.SaveChangesAsync();
            _logger.LogInformation("Offer created with OfferId {OfferId}", offer.OfferId);
            
            // Log the event instead of publishing via MassTransit
            _logger.LogInformation("OfferCreated event: OfferId {OfferId}, SellerId {SellerId}, VIN {VIN}, CreatedAt {CreatedAt}", 
                offer.OfferId, offer.SellerId, offer.VIN, offer.CreatedAt);
            
            return offer.OfferId;
        }

        public async Task UpdateOfferAsync(UpdateOfferRequestDto request)
        {
            _logger.LogInformation("Updating offer with OfferId {OfferId}", request.OfferId);

            var existingOffer = await _offerRepository.GetByIdAsync(request.OfferId);
            if (existingOffer == null)
            {
                throw new InvalidOperationException($"Offer with ID {request.OfferId} not found.");
            }

            existingOffer.OfferAmount = request.OfferAmount;
            existingOffer.City = request.City;
            existingOffer.State = request.State;
            existingOffer.Country = request.Country;
            existingOffer.Status = request.Status;
            existingOffer.UpdatedAt = request.UpdatedAt;

            await _offerRepository.UpdateAsync(existingOffer);
            await _offerRepository.SaveChangesAsync();

            _logger.LogInformation("Offer with OfferId {OfferId} updated successfully", request.OfferId);

            // Publish OfferUpdatedEvent
            var offerUpdatedEvent = new OfferUpdatedEvent
            {
                OfferId = existingOffer.OfferId,
                SellerId = existingOffer.SellerId,
                VIN = existingOffer.VIN,
                OfferAmount = existingOffer.OfferAmount,
                City = existingOffer.City,
                State = existingOffer.State,
                Country = existingOffer.Country,
                Status = existingOffer.Status,
                UpdatedAt = existingOffer.UpdatedAt
            };

            await _publisher.Publish(offerUpdatedEvent);

            _logger.LogInformation("Published OfferUpdatedEvent for OfferId {OfferId}", existingOffer.OfferId);
        }
    }
}
