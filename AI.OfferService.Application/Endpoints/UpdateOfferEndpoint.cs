using FastEndpoints;
using AI.OfferService.Application.DTOs;
using AI.OfferService.Application.Events;
using AI.OfferService.Application.Services;
using AI.OfferService.Domain.Entities;
using AI.OfferService.Domain.Repositories;

namespace AI.OfferService.Application.Endpoints;

public class UpdateOfferEndpoint : Endpoint<UpdateOfferRequest, OfferResponse>
{
    private readonly IOfferRepository _offerRepository;
    private readonly IEventPublisher _eventPublisher;

    public UpdateOfferEndpoint(IOfferRepository offerRepository, IEventPublisher eventPublisher)
    {
        _offerRepository = offerRepository;
        _eventPublisher = eventPublisher;
    }

    public override void Configure()
    {
        Put("/offers/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateOfferRequest req, CancellationToken ct)
    {
        var id = Route<long>("id");
        var existing = await _offerRepository.GetByIdAsync(id);

        if (existing == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var offer = new Offer
        {
            Id = id,
            BuyerId = req.BuyerId ?? existing.BuyerId,
            CarrierId = req.CarrierId ?? existing.CarrierId,
            OfferAmount = req.OfferAmount ?? existing.OfferAmount,
            City = req.City ?? existing.City,
            State = req.State ?? existing.State,
            Country = req.Country ?? existing.Country,
            Status = req.Status ?? existing.Status
        };

        var updated = await _offerRepository.UpdateAsync(offer);

        if (updated == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        // Publish OfferUpdated event
        var offerEvent = new OfferUpdatedEvent
        {
            OfferId = updated.Id,
            VehicleId = updated.VehicleId,
            SellerId = updated.SellerId,
            BuyerId = updated.BuyerId,
            CarrierId = updated.CarrierId,
            OfferAmount = updated.OfferAmount,
            City = updated.City,
            State = updated.State,
            Country = updated.Country,
            Status = updated.Status,
            UpdatedAt = updated.LastModifiedAt,
            Make = updated.Vehicle?.Make ?? string.Empty,
            Model = updated.Vehicle?.Model ?? string.Empty,
            Year = updated.Vehicle?.Year ?? 0,
            Trim = updated.Vehicle?.Trim ?? string.Empty,
            VIN = updated.Vehicle?.VIN ?? string.Empty
        };

        await _eventPublisher.PublishAsync(offerEvent, "offer.updated");

        var response = new OfferResponse
        {
            Id = updated.Id,
            VehicleId = updated.VehicleId,
            SellerId = updated.SellerId,
            BuyerId = updated.BuyerId,
            CarrierId = updated.CarrierId,
            OfferAmount = updated.OfferAmount,
            City = updated.City,
            State = updated.State,
            Country = updated.Country,
            Status = updated.Status,
            CreatedAt = updated.CreatedAt,
            LastModifiedAt = updated.LastModifiedAt,
            Vehicle = updated.Vehicle != null ? new VehicleInfo
            {
                Id = updated.Vehicle.Id,
                Make = updated.Vehicle.Make,
                Model = updated.Vehicle.Model,
                Year = updated.Vehicle.Year,
                Trim = updated.Vehicle.Trim,
                VIN = updated.Vehicle.VIN
            } : null
        };

        await SendAsync(response, cancellation: ct);
    }
}