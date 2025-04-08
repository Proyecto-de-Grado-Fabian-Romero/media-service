namespace MediaService.Src.Application.Interfaces;

public interface IStorageService
{
    Task<object> UploadImageAsync(IFormFile file, string bucketKey, string folderName);

    Task<bool> DeleteImageAsync(string bucketKey, string fileId, string fileName);
}
