using System;

namespace AI.OfferService.Models
{
    public class Offer
    {
        public long OfferId { get; set; }
        public long SellerId { get; set; }
        public string VIN { get; set; } = null!;
        public decimal? OfferAmount { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int VehicleId { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Vehicle Vehicle { get; set; } = null!;
    }
}
