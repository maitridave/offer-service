namespace AI.OfferService.Application.Events;

public class OfferCreatedEvent
{
    public long OfferId { get; set; }
    public long VehicleId { get; set; }
    public long SellerId { get; set; }
    public long BuyerId { get; set; }
    public long CarrierId { get; set; }
    public decimal? OfferAmount { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Vehicle details
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public string? Trim { get; set; }
    public string VIN { get; set; } = string.Empty;
    
    public string EventType => "OfferCreated";
}