namespace AI.OfferService.Application.DTOs;

public class UpdateOfferRequest
{
    public long? BuyerId { get; set; }
    public long? SellerId { get; set; }
    public long? CarrierId { get; set; }
    public decimal? OfferAmount { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? Status { get; set; }
}