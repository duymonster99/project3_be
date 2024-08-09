namespace CompanyServices.Helper
{
    public class FileUpload
    {
        private static readonly string _baseFolder = "AdminImages";
        private static readonly string _rootUrl = "http://localhost:5082";

        public static async Task<string> SaveImageAsync(string subFolder, IFormFile? formFile)
        {
            try
            {
                var imageName = $"{Guid.NewGuid()}_{formFile.FileName}";
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), _baseFolder, subFolder);

                if (!Directory.Exists(imagePath))
                {
                    Directory.CreateDirectory(imagePath);
                }

                var exactFilePath = Path.Combine(imagePath, imageName);
                await using (var fileStream = new FileStream(exactFilePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(fileStream);
                }

                return $"{_rootUrl}/{_baseFolder.Replace("\\", "/")}/{subFolder.Replace("\\", "/")}/{imageName.Replace("\\", "/")}";
            }
            catch (Exception ex)
            {
                // Log exception here
                throw new Exception("An error occurred while saving the image.", ex);
            }
        }

        public static void DeleteImage(string imagePath)
        {
            try
            {
                var exactPath = imagePath.Substring(_rootUrl.Length).TrimStart('/');
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), exactPath);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                // Log exception here
                throw new Exception("An error occurred while deleting the image.", ex);
            }
        }
    }
}
