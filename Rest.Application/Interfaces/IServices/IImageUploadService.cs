namespace Rest.Application.Interfaces.IServices
{
    public interface IImageUploadService
    {
        Task<string> UploadAsync(Stream fileStream, string originalFileName, long fileLength, string folder);
        void Delete(string? relativeUrl);
    }
}
