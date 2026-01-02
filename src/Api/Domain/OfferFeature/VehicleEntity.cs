using System;

namespace AI.OfferService.Domain.OfferFeature
{
    public class VehicleEntity 
    {
        public long Id { get; set; } // bigint PRIMARY KEY
        public string Make { get; set; } = string.Empty; // VARCHAR(50)
        public string Model { get; set; } = string.Empty; // VARCHAR(100)
        public int Year { get; set; } // INT
        public string Trim { get; set; } = string.Empty; // VARCHAR(100)
        public string VIN { get; set; } = string.Empty; // VARCHAR(17) NOT NULL
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
    }
}
