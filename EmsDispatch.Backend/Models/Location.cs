using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EmsDispatch.Backend.Models;

[BsonIgnoreExtraElements]
public class Location
{
    [BsonElement("latitude")]
    public double Latitude { get; set; }

    [BsonElement("longitude")]
    public double Longitude { get; set; }

    [BsonElement("address")]
    public string? Address { get; set; }

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
