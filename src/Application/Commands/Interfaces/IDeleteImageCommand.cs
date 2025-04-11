namespace MediaService.Src.Application.Commands.Interfaces;

public interface IDeleteImageCommand
{
    Task<bool> DeleteAsync(string bucketKey, string fileId, string fileName);
}
