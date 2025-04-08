namespace MediaService.Src.WebApi.Controllers;

using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class MediaController(IStorageService storageService) : ControllerBase
{
    private readonly IStorageService _storageService = storageService;

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(
    [FromForm] IFormFile file,
    [FromQuery] string bucket = "spacio",
    [FromQuery] string folder = "environments")
    {
        if (file == null || string.IsNullOrEmpty(bucket))
        {
            return BadRequest("File and bucket name are required.");
        }

        try
        {
            var mediaInfo = await _storageService.UploadImageAsync(file, bucket, folder);
            return Ok(new { mediaInfo });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("upload-multiple")]
    public async Task<IActionResult> UploadMultiple(
    [FromForm] List<IFormFile> files,
    [FromQuery] string bucket = "spacio",
    [FromQuery] string folder = "environments")
    {
        if (files == null || files.Count == 0)
        {
            return BadRequest("At least one file is required.");
        }

        var mediaInfos = new List<object>();

        try
        {
            foreach (var file in files)
            {
                var mediaInfo = await _storageService.UploadImageAsync(file, bucket, folder);
                mediaInfos.Add(mediaInfo);
            }

            return Ok(new { mediaInfos });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteImage(
        [FromQuery] string fileId,
        [FromQuery] string fileName,
        [FromQuery] string bucket = "spacio")
    {
        if (string.IsNullOrEmpty(fileId) || string.IsNullOrEmpty(fileName))
        {
            return BadRequest("FileId and fileName are required.");
        }

        try
        {
            var result = await _storageService.DeleteImageAsync(bucket, fileId, fileName);
            return Ok(new { message = "Image Deleted Successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
