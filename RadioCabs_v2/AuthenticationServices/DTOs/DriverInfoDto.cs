namespace AuthenticationServices.DTOs;

public class DriverInfoDto
{
    public string? DriverFullName { get; set; }
    public string? DriverMobile { get; set; }
    public string? DriverCode { get; set; }
    public string? DriverEmail { get; set; }
    public string? DriverLicense { get; set; }
    public string? Password { get; set; }
    public bool? IsActive { get; set; }
    public string? Address { get; set; }
    public string? Ward { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? RegistrationCar { get; set; }
    public IFormFile? DriverPersonalImage { get; set; }
    public IFormFile? DriverLicenseImage { get; set; }
}