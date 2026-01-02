namespace AI.OfferService.Domain.Entities;

public class Offer
{
    public long Id { get; set; }
    public long VehicleId { get; set; }
    public long SellerId { get; set; }
    public long BuyerId { get; set; }
    public long CarrierId { get; set; }
    public decimal? OfferAmount { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? Status { get; set; } // OPEN, SOLD, CANCELLED
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
    
    public virtual Vehicle? Vehicle { get; set; }
}