namespace MediaService.Src.WebApi.Controllers;

using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly IStorageService _storageService;

    public UploadController(IStorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpPost]
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
            var url = await _storageService.UploadImageAsync(file, bucket, folder);
            return Ok(new { url });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
