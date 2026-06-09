using HQVerse.Domain.Enums;

namespace HQVerse.Domain.Entities;

public class Media
{
    public int Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public MediaType Type { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = false;
}