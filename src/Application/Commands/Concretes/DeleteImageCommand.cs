namespace MediaService.src.Application.Commands.Concretes;

using MediaService.Src.Application.Commands.Interfaces;
using MediaService.Src.Application.Interfaces;

public class DeleteImageCommand(IStorageService storageService) : IDeleteImageCommand
{
    private readonly IStorageService _storageService = storageService;

    public async Task<bool> DeleteAsync(string bucket, string fileId, string fileName)
    {
        if (string.IsNullOrEmpty(fileId))
        {
            throw new ArgumentException("File ID cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException("File name cannot be null or empty.");
        }

        return await _storageService.DeleteImageAsync(bucket, fileId, fileName);
    }
}
