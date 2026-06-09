namespace HQVerse.Application.DTOs.Publishers;

public class PublisherDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Country { get; set; }
    public DateTime? FoundationDate { get; set; }
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePublisherDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Country { get; set; }
    public DateTime? FoundationDate { get; set; }
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
}

public class UpdatePublisherDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Country { get; set; }
    public DateTime? FoundationDate { get; set; }
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
}