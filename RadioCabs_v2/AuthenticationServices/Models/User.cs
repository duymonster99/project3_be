using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationServices.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string FullName { get; set; }
    
    
    public string Email { get; set; }
    
    public string Password { get; set; }
    public string? Role { get; set; }
    public bool? Status { get; set; }
    
    public virtual UserInfo? UserInfo { get; set; }
    
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    
}