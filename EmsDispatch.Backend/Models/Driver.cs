using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using EmsDispatch.Backend.Models.Enums;

namespace EmsDispatch.Backend.Models;

[BsonIgnoreExtraElements]
public class Driver
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("user_id")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("license_number")]
    public string LicenseNumber { get; set; } = string.Empty;

    [BsonElement("ambulance_id")]
    public string? AmbulanceId { get; set; }

    [BsonElement("current_location")]
    public Location? CurrentLocation { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public DriverStatus Status { get; set; }

    [BsonElement("experience_years")]
    public int ExperienceYears { get; set; }

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
