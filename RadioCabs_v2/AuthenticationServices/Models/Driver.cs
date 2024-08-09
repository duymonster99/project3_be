﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationServices.Models;

public class Driver
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string? DriverFullName { get; set; }
    public string? DriverCode { get; set; }
    public string? DriverMobile { get; set; }
    public string? DriverEmail { get; set; }
    public string? Password { get; set; }
    public bool? IsActive { get; set; }
    public string? Role { get; set; }
    public int? CompanyId { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? Rating { get; set; }
    public bool? IsOnline { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    
    public virtual DriverInfo? DriverInfo { get; set; }
    public virtual Booking? Booking { get; set; }
    public virtual ICollection<FeedbackDriver>? FeedbackDrivers { get; set; }
}