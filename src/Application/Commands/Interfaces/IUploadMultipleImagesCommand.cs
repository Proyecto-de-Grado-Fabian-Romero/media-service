namespace MediaService.Src.Application.Commands.Interfaces;

public interface IUploadMultipleImagesCommand
{
    Task<IEnumerable<object>> UploadMultipleAsync(List<IFormFile> files, string bucketKey, string folder);
}
