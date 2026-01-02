using System;

namespace AI.OfferService.Domain.OfferFeature
{
    public class UpdateOfferRequestDto
    {
        public long OfferId { get; set; }
        public decimal OfferAmount { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Status { get; set; } // OPEN, SOLD, CANCELLED
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
