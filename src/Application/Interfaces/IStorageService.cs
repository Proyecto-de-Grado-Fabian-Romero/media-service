namespace MediaService.Src.Application.Interfaces;

public interface IStorageService
{
    Task<string> UploadImageAsync(IFormFile file, string bucketKey, string folderName);
}
