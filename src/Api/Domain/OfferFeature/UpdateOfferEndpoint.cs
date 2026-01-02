using FastEndpoints;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace AI.OfferService.Domain.OfferFeature
{
    public class UpdateOfferEndpoint : Endpoint<UpdateOfferRequestDto>
    {
        private readonly IOfferService _offerService;
        private readonly ILogger<UpdateOfferEndpoint> _logger;

        public UpdateOfferEndpoint(IOfferService offerService, ILogger<UpdateOfferEndpoint> logger)
        {
            _offerService = offerService;
            _logger = logger;
        }

        public override void Configure()
        {
            Put("/offer");
            Version(1);
            DontCatchExceptions();
            Options(x => x
                .Produces(200)
                .Produces(400)
                .Produces(500));
        }

        public override async Task HandleAsync(UpdateOfferRequestDto req, CancellationToken ct)
        {
            _logger.LogInformation("Updating offer with OfferId {OfferId}", req.OfferId);
            await _offerService.UpdateOfferAsync(req);
            await SendOkAsync(ct);
        }
    }
}
