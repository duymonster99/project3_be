namespace CompanyServices.DTOs;

public class AdvertisementImageDto
{
    public int CompanyId { get; set; }
    public string? Description { get; set; }
    public IFormFile? ImageFile { get; set; }
}