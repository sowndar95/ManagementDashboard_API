using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace ManagementDashboard_Entities;

public abstract class BaseEntity
{
    public BaseEntity()
    {
        CollectionName = GetType().Name;
    }

    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }

    public Guid ModifiedBy { get; set; }
    public DateTimeOffset ModifiedDate { get; set; }

    [BsonIgnore]
    public string CollectionName { get; private set; }
}

[BsonIgnoreExtraElements]
public abstract class ManagementDashboardEntityBase
{
    public ManagementDashboardEntityBase()
    {
        CollectionName = GetType().Name;
    }

    public Guid Id { get; set; }
    public Guid ModifiedBy { get; set; }

    [BsonRepresentation(BsonType.Document)]
    public DateTimeOffset ModifiedDate { get; set; }

    [BsonIgnore]
    [JsonIgnore]
    public string CollectionName { get; private set; }
}
