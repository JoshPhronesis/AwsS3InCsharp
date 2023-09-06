using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Options;

namespace Customers.Api;

public class StorageService : IStorageService
{
    private readonly IAmazonS3 _amazonS3Client;
    private readonly IOptions<StorageConfig> _storageOptions;
    private readonly ILogger<StorageService> _logger;

    public StorageService(IAmazonS3 amazonS3Client, 
        IOptions<StorageConfig> storageOptions,
        ILogger<StorageService> logger)
    {
        _amazonS3Client = amazonS3Client;
        _storageOptions = storageOptions;
        _logger = logger;
    }

    public async Task<bool> RemoveImageAsync(Guid Id)
    {
        try
        {
            Guard.Against.Default(Id);
            var request = new DeleteObjectRequest()
            {
                BucketName = _storageOptions.Value.S3Bucket,
                Key = $"profile-picture/{Id}",
            };

            var response = await _amazonS3Client.DeleteObjectAsync(request);
            if (response.HttpStatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"an error has occured in {nameof(RemoveImageAsync)}");
        }

        return false;
    }

    public async Task<(Stream? responseStream, string contentType)> GetImageAsync(Guid Id)
    {
        try
        {
            Guard.Against.Default(Id);

            var request = new GetObjectRequest()
            {
                BucketName = _storageOptions.Value.S3Bucket,
                Key = $"profile-picture/{Id}",
            };

            var response = await _amazonS3Client.GetObjectAsync(request);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return (responseStream: response.ResponseStream, contentType: response.Headers.ContentType);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"an error has occured in {nameof(GetImageAsync)}");
        }

        return default;
    }

    public async Task<bool> UploadImageAsync(IFormFile image, Guid id)
    {
        try
        {
            Guard.Against.Null(image);
            Guard.Against.Default(id);

            var putObjectRequest = new PutObjectRequest()
            {
                BucketName = _storageOptions.Value.S3Bucket,
                Key = $"profile-picture/{id}",
                InputStream = image.OpenReadStream(),
                ContentType = image.ContentType,
                Metadata =
                {
                    ["x-amz-meta-original-file-name"] = image.FileName,
                    ["x-amz-meta-original-file-extension"] = Path.GetExtension(image.FileName),
                }
            };
            var response = await _amazonS3Client.PutObjectAsync(putObjectRequest);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return true;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"an error has occured in {nameof(UploadImageAsync)}");
        }

        return false;
    }
}