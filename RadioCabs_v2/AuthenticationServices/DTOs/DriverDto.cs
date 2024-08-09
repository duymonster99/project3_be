using AuthenticationServices.Database;

namespace AuthenticationServices.DTOs;

public class DriverDto
{
    public string DriverFullName { get; set; }
    public string DriverMobile { get; set; }
    public string Password { get; set; }
    public int CompanyId { get; set; }
    public decimal? Rating { get; set; }
    public string? Role { get; set; }
    public bool? IsOnline { get; set; }
}