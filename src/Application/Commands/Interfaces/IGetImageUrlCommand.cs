namespace MediaService.Src.Application.Commands.Interfaces;

public interface IGetImageUrlCommand
{
    Task<string> GetUrl(string bucket, string fileName);
}
