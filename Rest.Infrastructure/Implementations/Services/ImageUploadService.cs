using Rest.Application.Interfaces.IServices;
using Rest.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Infrastructure.Implementations.Services
{
    public class ImageUploadService : IImageUploadService
    {
        private readonly string _webRootPath;
        private static readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public ImageUploadService(string webRootPath)
        {
            _webRootPath = webRootPath;
        }

        public async Task<string> UploadAsync(Stream fileStream, string originalFileName, long fileLength, string folder)
        {
            if (fileStream == null || fileLength == 0)
                throw new ValidationException("No file was uploaded.");

            if (fileLength > MaxFileSizeBytes)
                throw new ValidationException("Image size cannot exceed 5 MB.");

            var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                throw new ValidationException($"Invalid image type. Allowed: {string.Join(", ", _allowedExtensions)}");

            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            if (!IsValidImageSignature(fileBytes, extension))
                throw new ValidationException("The file content does not match a valid image format.");

            var folderPath = Path.Combine(_webRootPath, "images", folder);
            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folderPath, fileName);

            await File.WriteAllBytesAsync(filePath, fileBytes);

            return $"/images/{folder}/{fileName}";
        }

        public void Delete(string? relativeUrl)
        {
            if (string.IsNullOrWhiteSpace(relativeUrl))
                return;

            var fullPath = Path.Combine(_webRootPath, relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        /// <summary>
        /// Verifies the file's actual binary signature matches its claimed
        /// extension — prevents renaming a malicious file to ".jpg" and
        /// bypassing the extension check.
        /// </summary>
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
