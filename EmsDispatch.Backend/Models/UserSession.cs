using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EmsDispatch.Backend.Models;

[BsonIgnoreExtraElements]
public class UserSession
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("user_id")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("login_at")]
    public DateTime LoginAt { get; set; }

    [BsonElement("logout_at")]
    public DateTime? LogoutAt { get; set; }

    [BsonElement("last_activity")]
    public DateTime LastActivity { get; set; }

    [BsonElement("is_online")]
    public bool IsOnline { get; set; }

    [BsonElement("ip_address")]
    public string? IpAddress { get; set; }

    [BsonElement("device_info")]
    public string? DeviceInfo { get; set; }
}
