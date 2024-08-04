namespace AuthenticationServices.Helper;

public class BlobContentTypes
{
    public static string GetContentType(IFormFile file)
    {
        // ToLowerInvariant 
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            _ => "application/octet-stream"
        };
    }
}