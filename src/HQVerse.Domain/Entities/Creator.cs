namespace HQVerse.Domain.Entities;

public class Creator
{
    public int Id { get; set; }
    public int? ComicVineId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string? Country { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    // Navigation properties
    public ICollection<IssueCreator> IssueCreators { get; set; } = new List<IssueCreator>();
}