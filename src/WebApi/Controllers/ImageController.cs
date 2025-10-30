namespace MediaService.Src.WebApi.Controllers;

using Application.Interfaces;
using MediaService.Src.Application.Commands.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ImageController(
        IStorageService storageService,
        IUploadImageCommand uploadCommand,
        IUploadMultipleImagesCommand uploadMultipleCommand,
        IDeleteImageCommand deleteCommand,
        IGetImageUrlCommand urlCommand) : ControllerBase
{
    private readonly IStorageService _storageService = storageService;
    private readonly IUploadImageCommand _uploadCommand = uploadCommand;
    private readonly IUploadMultipleImagesCommand _uploadMultipleCommand = uploadMultipleCommand;
    private readonly IDeleteImageCommand _deleteCommand = deleteCommand;
    private readonly IGetImageUrlCommand _urlCommand = urlCommand;

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(
    [FromForm] IFormFile file,
    [FromQuery] string bucket = "spacio",
    [FromQuery] string folder = "environments")
    {
        var result = await _uploadCommand.UploadAsync(file, bucket, folder);
        return CreatedAtAction(nameof(Upload), new { result }, result);
    }

    [HttpPost("upload-multiple")]
    public async Task<IActionResult> UploadMultiple(
        [FromForm] List<IFormFile> files,
        [FromQuery] string bucket = "spacio",
        [FromQuery] string folder = "environments")
    {
        var results = await _uploadMultipleCommand.UploadMultipleAsync(files, bucket, folder);
        return Ok(results);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(
        [FromQuery] string fileId,
        [FromQuery] string fileName,
        [FromQuery] string bucket = "spacio")
    {
        var deleted = await _deleteCommand.DeleteAsync(bucket, fileId, fileName);
        return deleted ? NoContent() : NotFound(new { message = "File not found" });
    }

    [HttpGet("url")]
    public IActionResult GetUrl(
        [FromQuery] string fileName,
        [FromQuery] string bucket = "spacio")
    {
        var url = _urlCommand.GetUrl(bucket, fileName);
        return Ok(new { url });
    }
}
