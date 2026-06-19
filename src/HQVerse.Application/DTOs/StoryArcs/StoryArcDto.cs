using HQVerse.Application.DTOs.ComicIssues;
using HQVerse.Application.DTOs.Publishers;

namespace HQVerse.Application.DTOs.StoryArcs;

public class StoryArcDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public PublisherDto? Publisher { get; set; }
    public int? PublisherId { get; set; }
    public int IssueCount { get; set; }
}

public class StoryArcDetailDto : StoryArcDto
{
    public List<ComicIssueDto> Issues { get; set; } = new();
}

public class CreateStoryArcDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? PublisherId { get; set; }
    public string? ImageUrl { get; set; }
}

public class UpdateStoryArcDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? PublisherId { get; set; }
    public string? ImageUrl { get; set; }
}

public class AddIssueToArcDto
{
    public int IssueId { get; set; }
    public int OrderNumber { get; set; }
}
