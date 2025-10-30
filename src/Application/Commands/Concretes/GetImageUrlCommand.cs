namespace MediaService.src.Application.Commands.Concretes;

using MediaService.Src.Application.Commands.Interfaces;
using MediaService.Src.Application.Interfaces;

public class GetImageUrlCommand : IGetImageUrlCommand
{
    private readonly IStorageService _storageService;

    public GetImageUrlCommand(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<string> GetUrl(string bucket, string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException("File name cannot be null or empty.");
        }

        return await _storageService.GetPresignedUrlAsync(bucket, fileName);
    }
}
