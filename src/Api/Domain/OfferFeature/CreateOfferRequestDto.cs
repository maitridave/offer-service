namespace AI.OfferService.Domain.OfferFeature
{
    public class CreateOfferRequestDto
    {
        public long SellerId { get; set; }
        public string VIN { get; set; } = string.Empty;
        public decimal OfferAmount { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Status { get; set; }
        public int Year { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Trim { get; set; } = string.Empty;
    }
}

