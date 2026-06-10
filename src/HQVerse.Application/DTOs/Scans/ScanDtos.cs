namespace HQVerse.Application.DTOs.Scans;

public class ScanGroupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    public string? Discord { get; set; }
    public string? Telegram { get; set; }
    public int ScanCount { get; set; }
}

public class ScanDto
{
    public int Id { get; set; }
    public int IssueId { get; set; }
    public string IssueTitle { get; set; } = string.Empty;
    public string SeriesName { get; set; } = string.Empty;
    public string IssueNumber { get; set; } = string.Empty;
    public string? CoverUrl { get; set; }
    public int? ScanGroupId { get; set; }
    public string? ScanGroupName { get; set; }
    public string? Version { get; set; }
    public string Language { get; set; } = "pt-BR";
    public int? Pages { get; set; }
    public long? FileSize { get; set; }
    public string? Format { get; set; }
    public string? Quality { get; set; }
    public int? UploaderUserId { get; set; }
    public string? UploaderUsername { get; set; }
    public List<ScanLinkDto> Links { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class ScanDetailDto : ScanDto
{
    public string? Synopsis { get; set; }
    public string? ISBN { get; set; }
    public string? UPC { get; set; }
    public int ReviewCount { get; set; }
    public double AverageRating { get; set; }
}

public class ScanLinkDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
}

public class CreateScanGroupDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    public string? Discord { get; set; }
    public string? Telegram { get; set; }
}

public class CreateScanDto
{
    public int IssueId { get; set; }
    public int? ScanGroupId { get; set; }
    public string? Version { get; set; }
    public string Language { get; set; } = "pt-BR";
    public int? Pages { get; set; }
    public long? FileSize { get; set; }
    public string? Format { get; set; }
    public string? Quality { get; set; }
    public List<CreateScanLinkDto> Links { get; set; } = new();
}

public class CreateScanLinkDto
{
    public string Type { get; set; } = "DOWNLOAD";
    public string Url { get; set; } = string.Empty;
    public bool IsOnline { get; set; } = true;
}

public class ScanFilterDto
{
    public string? Language { get; set; }
    public int? ScanGroupId { get; set; }
    public string? Format { get; set; }
    public string? Quality { get; set; }
    public string? SortBy { get; set; } = "createdAt";
    public string? SortOrder { get; set; } = "desc";
}