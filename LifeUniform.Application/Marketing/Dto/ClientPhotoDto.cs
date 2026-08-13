namespace LifeUniform.Application.Marketing.Dto;

public class ClientPhotoDto
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? ReviewText { get; set; }
    public int Rating { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}
