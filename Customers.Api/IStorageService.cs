namespace Customers.Api;

public  interface IStorageService
{
    Task<bool> RemoveImageAsync(Guid id);
    Task<(Stream? responseStream, string contentType)> GetImageAsync(Guid id);
    Task<bool> UploadImageAsync(IFormFile image, Guid id);
}