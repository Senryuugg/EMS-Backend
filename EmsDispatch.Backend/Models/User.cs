using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using EmsDispatch.Backend.Models.Enums;

namespace EmsDispatch.Backend.Models;

[BsonIgnoreExtraElements]
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("full_name")]
    public string FullName { get; set; } = string.Empty;

    [BsonElement("phone")]
    public string Phone { get; set; } = string.Empty;

    [BsonElement("role")]
    [BsonRepresentation(BsonType.String)]
    public UserRole Role { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public UserStatus Status { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("last_login")]
    public DateTime? LastLogin { get; set; }

    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;
}
