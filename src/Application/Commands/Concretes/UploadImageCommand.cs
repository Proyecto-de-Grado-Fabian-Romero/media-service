namespace MediaService.src.Application.Commands.Concretes;

using MediaService.Src.Application.Commands.Interfaces;
using MediaService.Src.Application.Interfaces;

public class UploadImageCommand(IStorageService storageService) : IUploadImageCommand
{
    private readonly IStorageService _storageService = storageService;

    public async Task<object> UploadAsync(IFormFile file, string bucketKey, string folder)
    {
        if (file == null)
        {
            throw new ArgumentException("File cannot be null.");
        }

        if (string.IsNullOrEmpty(bucketKey))
        {
            throw new ArgumentException("Bucket cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(folder))
        {
            folder = "default";
        }

        return await _storageService.UploadImageAsync(file, bucketKey, folder);
    }
}
