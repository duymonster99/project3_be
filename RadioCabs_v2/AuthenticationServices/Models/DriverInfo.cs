using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AuthenticationServices.Models;

public class DriverInfo
{
    [Key]
    [ForeignKey("Driver")]
    public int DriverId { get; set; }
    public string? DriverLicense { get; set; }
    public string? Address { get; set; }
    public string? Street { get; set; }
    public string? Ward { get; set; }
    public string? City { get; set; }
    public string? RegistrationCar { get; set; }
    public string? DriverPersonalImage { get; set; }
    public string? DriverLicenseImage { get; set; }
    
    [JsonIgnore]
    public virtual Driver? Driver { get; set; }
}