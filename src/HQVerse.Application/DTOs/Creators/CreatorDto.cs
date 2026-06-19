namespace HQVerse.Application.DTOs.Creators;

public class CreatorDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string? Country { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}

public class CreateCreatorDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string? Country { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}

public class UpdateCreatorDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string? Country { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}
