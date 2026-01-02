namespace AI.OfferService.Domain.Entities;

public class Vehicle
{
    public long Id { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public string? Trim { get; set; }
    public string VIN { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
    
    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();
}