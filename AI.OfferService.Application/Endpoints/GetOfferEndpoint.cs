using FastEndpoints;
using AI.OfferService.Application.DTOs;
using AI.OfferService.Domain.Repositories;

namespace AI.OfferService.Application.Endpoints;

public class GetOfferEndpoint : EndpointWithoutRequest<OfferResponse>
{
    private readonly IOfferRepository _offerRepository;

    public GetOfferEndpoint(IOfferRepository offerRepository)
    {
        _offerRepository = offerRepository;
    }

    public override void Configure()
    {
        Get("/offers/{id}");
        AllowAnonymous();
        Summary(s => {
            s.Summary = "Get an offer by ID";
            s.Description = "Retrieves a specific offer using its unique identifier";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");
        var offer = await _offerRepository.GetByIdAsync(id);

        if (offer == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

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
            Vehicle = offer.Vehicle != null ? new VehicleInfo
            {
                Id = offer.Vehicle.Id,
                Make = offer.Vehicle.Make,
                Model = offer.Vehicle.Model,
                Year = offer.Vehicle.Year,
                Trim = offer.Vehicle.Trim,
                VIN = offer.Vehicle.VIN
            } : null
        };

        await SendAsync(response, cancellation: ct);
    }
}