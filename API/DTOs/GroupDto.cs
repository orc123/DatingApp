namespace API.DTOs;

public class GroupDto
{
    public string Name { get; set; }
    // nav property
    public ICollection<ConnectionDto> Connections { get; set; } = [];
}
