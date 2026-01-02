using FastEndpoints;

namespace AI.OfferService.Domain.OfferFeature
{
    public class CreateOfferEndpoint : Endpoint<CreateOfferRequestDto, CreateOfferResponseDto>
    {
        private readonly IOfferService _offerService;
        private readonly ILogger<CreateOfferEndpoint> _logger;

        public CreateOfferEndpoint(IOfferService offerService, ILogger<CreateOfferEndpoint> logger)
        {
            _offerService = offerService;
            _logger = logger;
        }

        public override void Configure()
        {
            Post("/offer");
            Version(1);
            DontCatchExceptions();
            Options(x => x
                .Produces<CreateOfferResponseDto>()
                .Produces(200)
                .Produces(304)
                .Produces(500));
        }

        public override async Task HandleAsync(CreateOfferRequestDto req, CancellationToken ct)
        {
            _logger.LogInformation("Received CreateOffer request: {@Request}", req);
            var offerId = await _offerService.CreateOfferAsync(req);
            var response = new CreateOfferResponseDto
            {
                OfferId = offerId,
                Message = "Offer created successfully."
            };
            await SendAsync(response);
        }
    }
}
