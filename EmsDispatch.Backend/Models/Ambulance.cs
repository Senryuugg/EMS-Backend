using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using EmsDispatch.Backend.Models.Enums;

namespace EmsDispatch.Backend.Models;

[BsonIgnoreExtraElements]
public class Ambulance
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("registration_number")]
    public string RegistrationNumber { get; set; } = string.Empty;

    [BsonElement("driver_id")]
    public string? DriverId { get; set; }

    [BsonElement("current_location")]
    public Location? CurrentLocation { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public DriverStatus Status { get; set; }

    [BsonElement("capacity")]
    public int Capacity { get; set; }

    [BsonElement("equipment")]
    public List<string> Equipment { get; set; } = new();

    [BsonElement("base_id")]
    public string? BaseId { get; set; }

    [BsonElement("active_dispatch_id")]
    public string? ActiveDispatchId { get; set; }

    [BsonElement("total_dispatches")]
    public int TotalDispatches { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;
}
