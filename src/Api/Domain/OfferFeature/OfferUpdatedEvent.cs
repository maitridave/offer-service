using System;

namespace AI.OfferService.Domain.OfferFeature
{
    public class OfferUpdatedEvent
    {
        public long OfferId { get; set; }
        public long SellerId { get; set; }
        public string VIN { get; set; } = string.Empty;
        public decimal OfferAmount { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
