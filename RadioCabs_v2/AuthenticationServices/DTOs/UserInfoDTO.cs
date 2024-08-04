namespace AuthenticationServices.DTOs;

public class UserInfoDTO
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public bool? Status { get; set; }
    public string? Mobile { get; set; }
    public string? Address { get; set; }
    public string? Street { get; set; }
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? Location { get; set; }
    public IFormFile? PersonalImage { get; set; }
    public string? Role { get; set; }
}