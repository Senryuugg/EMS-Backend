using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using EmsDispatch.Backend.Models.Enums;

namespace EmsDispatch.Backend.Models;

[BsonIgnoreExtraElements]
public class Dispatch
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("call_id")]
    public string CallId { get; set; } = string.Empty;

    [BsonElement("patient_info")]
    public PatientInfo PatientInfo { get; set; } = new();

    [BsonElement("priority")]
    [BsonRepresentation(BsonType.String)]
    public Priority Priority { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public DispatchStatus Status { get; set; }

    [BsonElement("pickup_location")]
    public Location PickupLocation { get; set; } = new();

    [BsonElement("assigned_driver_id")]
    public string? AssignedDriverId { get; set; }

    [BsonElement("assigned_ambulance_id")]
    public string? AssignedAmbulanceId { get; set; }

    [BsonElement("hospital_prediction")]
    public HospitalPrediction? HospitalPrediction { get; set; }

    [BsonElement("created_by_id")]
    public string CreatedById { get; set; } = string.Empty;

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [BsonElement("notes")]
    public string Notes { get; set; } = string.Empty;
}

[BsonIgnoreExtraElements]
public class HospitalPrediction
{
    [BsonElement("hospital_id")]
    public string HospitalId { get; set; } = string.Empty;

    [BsonElement("hospital_name")]
    public string HospitalName { get; set; } = string.Empty;

    [BsonElement("confidence_score")]
    public double ConfidenceScore { get; set; }

    [BsonElement("reason")]
    public string Reason { get; set; } = string.Empty;

    [BsonElement("estimated_distance_km")]
    public double EstimatedDistanceKm { get; set; }

    [BsonElement("estimated_arrival_minutes")]
    public int EstimatedArrivalMinutes { get; set; }
}
