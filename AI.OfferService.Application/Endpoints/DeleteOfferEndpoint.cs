using FastEndpoints;
using AI.OfferService.Domain.Repositories;

namespace AI.OfferService.Application.Endpoints;

public class DeleteOfferEndpoint : EndpointWithoutRequest
{
    private readonly IOfferRepository _offerRepository;

    public DeleteOfferEndpoint(IOfferRepository offerRepository)
    {
        _offerRepository = offerRepository;
    }

    public override void Configure()
    {
        Delete("/offers/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");
        var success = await _offerRepository.DeleteAsync(id);

        if (!success)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendNoContentAsync(ct);
    }
}