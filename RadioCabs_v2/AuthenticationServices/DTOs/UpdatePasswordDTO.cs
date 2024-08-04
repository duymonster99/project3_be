namespace AuthenticationServices.DTOs;

public class UpdatePasswordDTO
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}