using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace CompanyServices.Models;

public class Company
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    // login

    public string? CompanyName { get; set; }

    public string? CompanyTaxCode { get; set; }

    public string? CompanyEmail { get; set; }

    public string? CompanyPassword { get; set; }
    public string? Role { get; set; }
    
    // Update 
    public string? ContactPerson { get; set; }
    public string? Designation { get; set; }
    public string? ContactPersonMobile { get; set; }
    public string? CompanyTelephone { get; set; }
    public string? CompanyImageUrl { get; set; }
    public string? CompanyAddress { get; set; }
    public string? CompanyWard { get; set; }
    public string? CompanyDistrict { get; set; }
    public string? CompanyCity { get; set; }
    
    public bool? IsActive { get; set; }
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }
    
    // Service Reference
    public string? MembershipType { get; set; } //
    
    // TOKEN
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    
    // RELATIONSHIP
    public virtual ICollection<CompanyService>? CompanyServices { get; set; }
    public virtual ICollection<CompanyLocationService> CompanyLocationServices { get; set; }
    public virtual ICollection<AdvertisementImage>? Advertisements { get; set; }
    public Payment? Payments { get; set; } //
}