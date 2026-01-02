using FastEndpoints;
using AI.OfferService.Application.DTOs;
using AI.OfferService.Application.Events;
using AI.OfferService.Application.Services;
using AI.OfferService.Domain.Entities;
using AI.OfferService.Domain.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AI.OfferService.Application.Endpoints;

public class CreateOfferEndpoint : Endpoint<CreateOfferRequest, OfferResponse>
{
    private readonly IOfferRepository _offerRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IEventPublisher _eventPublisher;

    public CreateOfferEndpoint(
        IOfferRepository offerRepository,
        IVehicleRepository vehicleRepository,
        IEventPublisher eventPublisher)
    {
        _offerRepository = offerRepository;
        _vehicleRepository = vehicleRepository;
        _eventPublisher = eventPublisher;
    }

    public override void Configure()
    {
        Post("/offers");
        AllowAnonymous();
        Summary(s => {
            s.Summary = "Create a new offer";
            s.Description = "Creates a new vehicle offer with seller, buyer, and carrier information";
            s.ExampleRequest = new CreateOfferRequest
            {
                VIN = "1HGCM82633A123456",
                Make = "Honda",
                Model = "Accord",
                Year = 2023,
                SellerId = 1,
                BuyerId = 2,
                CarrierId = 3,
                OfferAmount = 25000.00m
            };
        });
    }

    public override async Task HandleAsync(CreateOfferRequest req, CancellationToken ct)
    {
        // Debug logging to see what we received
        Console.WriteLine($"Received: SellerId={req.SellerId}, BuyerId={req.BuyerId}, CarrierId={req.CarrierId}, Amount={req.OfferAmount}");
        
        // Check if vehicle exists by VIN
        var vehicle = await _vehicleRepository.GetByVINAsync(req.VIN);
        
        if (vehicle == null)
        {
            // Create new vehicle
            vehicle = new Vehicle
            {
                Make = req.Make,
                Model = req.Model,
                Year = req.Year,
                Trim = req.Trim,
                VIN = req.VIN
            };
            vehicle = await _vehicleRepository.CreateAsync(vehicle);
        }

        // Create offer
        var offer = new Offer
        {
            VehicleId = vehicle.Id,
            SellerId = req.SellerId,
            BuyerId = req.BuyerId,
            CarrierId = req.CarrierId,
            OfferAmount = req.OfferAmount,
            City = req.City,
            State = req.State,
            Country = req.Country,
            Status = "OPEN"
        };

        offer = await _offerRepository.CreateAsync(offer);

        // Publish event
        var offerEvent = new OfferCreatedEvent
        {
            OfferId = offer.Id,
            VehicleId = vehicle.Id,
            SellerId = offer.SellerId,
            BuyerId = offer.BuyerId,
            CarrierId = offer.CarrierId,
            OfferAmount = offer.OfferAmount,
            City = offer.City,
            State = offer.State,
            Country = offer.Country,
            Status = offer.Status,
            CreatedAt = offer.CreatedAt,
            Make = vehicle.Make,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Trim = vehicle.Trim,
            VIN = vehicle.VIN
        };

        await _eventPublisher.PublishAsync(offerEvent, "offer.created");

        var response = new OfferResponse
        {
            Id = offer.Id,
            VehicleId = offer.VehicleId,
            SellerId = offer.SellerId,
            BuyerId = offer.BuyerId,
            CarrierId = offer.CarrierId,
            OfferAmount = offer.OfferAmount,
            City = offer.City,
            State = offer.State,
            Country = offer.Country,
            Status = offer.Status,
            CreatedAt = offer.CreatedAt,
            LastModifiedAt = offer.LastModifiedAt,
            Vehicle = new VehicleInfo
            {
                Id = vehicle.Id,
                Make = vehicle.Make,
                Model = vehicle.Model,
                Year = vehicle.Year,
                Trim = vehicle.Trim,
                VIN = vehicle.VIN
            }
        };

        await SendAsync(response, 201, ct);
    }
}