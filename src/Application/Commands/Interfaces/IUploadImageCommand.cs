namespace MediaService.Src.Application.Commands.Interfaces;

public interface IUploadImageCommand
{
    Task<object> UploadAsync(IFormFile file, string bucketKey, string folder);
}
