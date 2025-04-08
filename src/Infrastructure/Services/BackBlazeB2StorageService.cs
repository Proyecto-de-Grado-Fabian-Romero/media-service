namespace MediaService.Src.Infrastructure.Services;

using System;
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

    public async Task<string> UploadImageAsync(IFormFile file, string bucketKey, string folder)
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

        return uploadedFile?.FileName ?? "Upload failed";
    }
}
