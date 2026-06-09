namespace HQVerse.Domain.Entities;

public class ExternalSource
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? BaseUrl { get; set; }
    public bool ApiKeyRequired { get; set; } = true;

    // Navigation properties
    public ICollection<ExternalMapping> Mappings { get; set; } = new List<ExternalMapping>();
}