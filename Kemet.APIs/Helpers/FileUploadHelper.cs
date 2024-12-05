namespace Kemet.APIs.Helpers
{
    public class FileUploadHelper
    {
        private readonly IWebHostEnvironment _environment;

        public FileUploadHelper(IWebHostEnvironment environment)
        {
            _environment = environment;
        }


        public async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file.");

            string uploadsFolder = Path.Combine(_environment.WebRootPath, folder);
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{folder}/{uniqueFileName}";
        }


    }
}
