namespace HQVerse.Domain.Entities;

public class CreatorRole
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<IssueCreator> IssueCreators { get; set; } = new List<IssueCreator>();
}