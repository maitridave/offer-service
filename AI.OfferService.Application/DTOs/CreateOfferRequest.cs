using System.Text.Json.Serialization;

namespace AI.OfferService.Application.DTOs;

public class CreateOfferRequest
{
    [JsonPropertyName("seller_id")]
    public long SellerId { get; set; }
    
    [JsonPropertyName("buyer_id")]
    public long BuyerId { get; set; }
    
    [JsonPropertyName("carrier_id")]
    public long CarrierId { get; set; }
    
    [JsonPropertyName("offer_amount")]
    public decimal? OfferAmount { get; set; }
    
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    
    // Vehicle details
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public string? Trim { get; set; }
    
    [JsonPropertyName("vin")]
    public string VIN { get; set; } = string.Empty;
}