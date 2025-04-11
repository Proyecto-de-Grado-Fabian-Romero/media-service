namespace MediaService.src.Application.Commands.Concretes;

using MediaService.Src.Application.Commands.Interfaces;
using MediaService.Src.Application.Interfaces;

public class UploadMultipleImagesCommand(IStorageService storageService) : IUploadMultipleImagesCommand
{
    private readonly IStorageService _storageService = storageService;

    public async Task<IEnumerable<object>> UploadMultipleAsync(List<IFormFile> files, string bucketKey, string folder)
    {
        if (files == null || files.Count == 0)
        {
            throw new ArgumentException("At least one file must be provided.");
        }

        var results = new List<object>();
        foreach (var file in files)
        {
            var result = await _storageService.UploadImageAsync(file, bucketKey, folder);
            results.Add(result);
        }

        return results;
    }
}