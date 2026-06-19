namespace HQVerse.Application.DTOs.Creators;

public class CreatorRoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreateCreatorRoleDto
{
    public string Name { get; set; } = string.Empty;
}
