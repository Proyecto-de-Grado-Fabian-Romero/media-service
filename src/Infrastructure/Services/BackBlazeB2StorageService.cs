namespace MediaService.Src.Infrastructure.Services;

using System;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using B2Net;
using B2Net.Models;
using MediaService.Src.Application.Interfaces;
using MediaService.Src.Infrastructure.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

public class BackblazeB2StorageService : IStorageService
{
    private readonly B2Client _b2Client;
    private readonly Dictionary<string, string> _bucketMap;

    public BackblazeB2StorageService(IOptions<StorageSettings> options, IOptions<BackBlazeCredentials> creds)
    {
        _bucketMap = options.Value.Buckets;

        var config = new B2Options
        {
            KeyId = creds.Value.KeyId,
            ApplicationKey = creds.Value.ApplicationKey,
        };

        _b2Client = new B2Client(config);
    }

    public async Task<object> UploadImageAsync(IFormFile file, string bucketKey, string folder)
    {
        if (!_bucketMap.TryGetValue(bucketKey, out string? bucketName))
        {
            throw new Exception("Invalid bucket key");
        }

        var buckets = await _b2Client.Buckets.GetList();
        var bucket = buckets.FirstOrDefault(b => b.BucketName == bucketName) ?? throw new Exception("Bucket not found");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        byte[] fileBytes = memoryStream.ToArray();

        var safeFolder = folder.Trim('/');
        var fileName = $"{safeFolder}/{Guid.NewGuid()}_{file.FileName}";

        var uploadedFile = await _b2Client.Files.Upload(fileBytes, fileName, bucket.BucketId);

        if (uploadedFile != null)
        {
            return new { uploadedFile.FileId, FileName = fileName };
        }

        return new { Message = "Upload failed" };
    }

    public async Task<bool> DeleteImageAsync(string bucketKey, string fileId, string fileName)
    {
        if (!_bucketMap.TryGetValue(bucketKey, out string? bucketName))
        {
            throw new Exception("Invalid bucket key");
        }

        try
        {
            var buckets = await _b2Client.Buckets.GetList();
            var bucket = buckets.FirstOrDefault(b => b.BucketName == bucketName) ?? throw new Exception("Bucket not found");

            await _b2Client.Files.Delete(fileId, fileName);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting file: {ex.Message}");
        }
    }

    public async Task<string> GetPresignedUrlAsync(string bucketKey, string fileName, int validDurationInSeconds = 7200)
    {
        if (!_bucketMap.TryGetValue(bucketKey, out string? bucketName))
        {
            throw new Exception("Invalid bucket key");
        }

        var buckets = await _b2Client.Buckets.GetList();
        var bucket = buckets.FirstOrDefault(b => b.BucketName == bucketName) ?? throw new Exception("Bucket not found");

        var authResponse = await _b2Client.Authorize();
        var apiUrl = authResponse.ApiUrl;
        var downloadUrl = authResponse.DownloadUrl;
        var authToken = authResponse.AuthorizationToken;

        var requestBody = new
        {
            bucketId = bucket.BucketId,
            fileNamePrefix = fileName,
            validDurationInSeconds,
        };

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authToken);

        var requestJson = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{apiUrl}/b2api/v2/b2_get_download_authorization", requestJson);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to get download token: {await response.Content.ReadAsStringAsync()}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        var parsed = JsonDocument.Parse(responseJson);
        var downloadToken = parsed.RootElement.GetProperty("authorizationToken").GetString();

        var presignedUrl = $"{downloadUrl}/file/{bucketName}/{fileName}?Authorization={downloadToken}";

        return presignedUrl;
    }
}
