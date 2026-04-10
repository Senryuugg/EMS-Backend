using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EmsDispatch.Backend.Models;

[BsonIgnoreExtraElements]
public class PatientInfo
{
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("age")]
    public int Age { get; set; }

    [BsonElement("gender")]
    public string Gender { get; set; } = string.Empty;

    [BsonElement("medical_condition")]
    public string MedicalCondition { get; set; } = string.Empty;

    [BsonElement("blood_type")]
    public string BloodType { get; set; } = string.Empty;

    [BsonElement("allergies")]
    public List<string> Allergies { get; set; } = new();

    [BsonElement("medications")]
    public List<string> Medications { get; set; } = new();

    [BsonElement("notes")]
    public string Notes { get; set; } = string.Empty;
}
