using System.Text.RegularExpressions;

namespace AuthenticationServices.Helper;

public class CheckingPattern
{
    // Check EMAIL pattern 
    public static bool IsEmail(string email)
    {
        return Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
    }
    
    // Check MOBILE pattern 
    public static bool IsMobile(string mobile)
    {
        return Regex.IsMatch(mobile, @"^(\d{10})$");
    }
    
    // Add +84 to MOBILE
    public static string AddPrefixMobile(string mobile)
    {
        return $"+84{mobile}";
    }
    
    // Remove +84 from MOBILE
    public static string RemovePrefixMobile(string mobile)
    {
        return mobile.Substring(3);
    }
    
    // Check License pattern - if USER have STRING in LICENSE 
    public static bool IsLicense(string license)
    {
        return Regex.IsMatch(license, @"[a-zA-Z]");
    }
    
   
}