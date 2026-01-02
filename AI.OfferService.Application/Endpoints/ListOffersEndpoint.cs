using FastEndpoints;
using AI.OfferService.Application.DTOs;
using AI.OfferService.Domain.Repositories;

namespace AI.OfferService.Application.Endpoints;

public class ListOffersRequest
{
    public long? SellerId { get; set; }
}

public class ListOffersEndpoint : Endpoint<ListOffersRequest, List<OfferResponse>>
{
    private readonly IOfferRepository _offerRepository;

    public ListOffersEndpoint(IOfferRepository offerRepository)
    {
        _offerRepository = offerRepository;
    }

    public override void Configure()
    {
        Get("/offers");
        AllowAnonymous();
        Summary(s => {
            s.Summary = "List all offers";
            s.Description = "Retrieves a list of offers, optionally filtered by seller ID";
            s.ExampleRequest = new ListOffersRequest { SellerId = 1 };
        });
    }

    public override async Task HandleAsync(ListOffersRequest req, CancellationToken ct)
    {
        var offers = await _offerRepository.GetAllAsync(req.SellerId);

        var response = offers.Select(offer => new OfferResponse
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
        }).ToList();

        await SendAsync(response, cancellation: ct);
    }
}