using Rest.Application.Interfaces.IServices;
using Rest.Application.Validation;
using Rest.Domain.Exceptions;

namespace Rest.Infrastructure.Implementations.Services
{
    public class ImageUploadService : IImageUploadService
    {
        private readonly string _webRootPath;
        private static readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private static readonly string[] _allowedFolders = { "products", "users" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public ImageUploadService(string webRootPath)
        {
            _webRootPath = webRootPath;
        }

        public async Task<string> UploadAsync(Stream fileStream, string originalFileName, long fileLength, string folder)
        {
            if (!_allowedFolders.Contains(folder))
                throw new ValidationException("Invalid folder.");

            if (fileStream == null || fileLength == 0)
                throw new ValidationException("No file was uploaded.");

            if (fileLength > MaxFileSizeBytes)
                throw new ValidationException("Image size cannot exceed 5 MB.");

            var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                throw new ValidationException($"Invalid image type. Allowed: {string.Join(", ", _allowedExtensions)}");

            // never trust the declared length: read at most MaxFileSizeBytes + 1
            using var memoryStream = new MemoryStream();
            var buffer = new byte[81920];
            int read;
            while ((read = await fileStream.ReadAsync(buffer)) > 0)
            {
                await memoryStream.WriteAsync(buffer.AsMemory(0, read));
                if (memoryStream.Length > MaxFileSizeBytes)
                    throw new ValidationException("Image size cannot exceed 5 MB.");
            }
            var fileBytes = memoryStream.ToArray();

            if (!IsValidImageSignature(fileBytes, extension))
                throw new ValidationException("The file content does not match a valid image format.");

            var folderPath = Path.Combine(_webRootPath, "images", folder);
            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{extension}";
            await File.WriteAllBytesAsync(Path.Combine(folderPath, fileName), fileBytes);

            return $"/images/{folder}/{fileName}";
        }

        /// <summary>
        /// Deletes ONLY files that (1) match the URL shape we generate and
        /// (2) resolve to a path inside wwwroot/images. Anything else is ignored.
        /// </summary>
        public void Delete(string? relativeUrl)
        {
            if (!TryResolvePath(relativeUrl, out var fullPath)) return;
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }

        private bool TryResolvePath(string? url, out string fullPath)
        {
            fullPath = string.Empty;
            if (!ImageUrlRules.IsValid(url)) return false;

            var imagesRoot = Path.GetFullPath(Path.Combine(_webRootPath, "images"))
                             + Path.DirectorySeparatorChar;

            var candidate = Path.GetFullPath(Path.Combine(
                _webRootPath,
                url!.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)));

            if (!candidate.StartsWith(imagesRoot, StringComparison.OrdinalIgnoreCase)) return false;

            fullPath = candidate;
            return true;
        }

        private static bool IsValidImageSignature(byte[] bytes, string extension)
        {
            if (bytes.Length < 12) return false;

            return extension switch
            {
                ".png" => bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47,
                ".jpg" or ".jpeg" => bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF,
                ".webp" => bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46
                        && bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50,
                _ => false
            };
        }
    }
}
