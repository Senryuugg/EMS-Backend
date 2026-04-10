using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EmsDispatch.Backend.Models;

[BsonIgnoreExtraElements]
public class Hospital
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("location")]
    public Location Location { get; set; } = new();

    [BsonElement("phone")]
    public string Phone { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("specialties")]
    public List<string> Specialties { get; set; } = new();

    [BsonElement("capacity")]
    public int Capacity { get; set; }

    [BsonElement("current_load")]
    public int CurrentLoad { get; set; }

    [BsonElement("rating")]
    public double Rating { get; set; }

    [BsonElement("address")]
    public string Address { get; set; } = string.Empty;

    [BsonElement("bed_count")]
    public int BedCount { get; set; }

    [BsonElement("icu_beds")]
    public int IcuBeds { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;
}
