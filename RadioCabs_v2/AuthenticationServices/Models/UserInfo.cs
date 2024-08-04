using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AuthenticationServices.Models;

public class UserInfo
{
    [Key]
    [ForeignKey("User")]
    public int UserId { get; set; }
    public string? Mobile { get; set; }
    public string? Address { get; set; }
    public string? Street { get; set; }
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? Location { get; set; }
    public string? Image { get; set; }
    [JsonIgnore]
    public virtual User? User  { get; set; }
}