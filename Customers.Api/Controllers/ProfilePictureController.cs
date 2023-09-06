using Microsoft.AspNetCore.Mvc;

namespace Customers.Api.Controllers;

[Route("api/user/{userId:guid}/profile-picture")]
[ApiController]
public class ProfilePictureController : ControllerBase
{
    private readonly IStorageService _storageService;

    public ProfilePictureController(IStorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpPost]
    public async Task<IActionResult> SavePicture([FromForm] IFormFile image, [FromRoute] Guid userId)
    {
        var result = await _storageService.UploadImageAsync(image, userId);
        return result switch
        {
            true => CreatedAtAction(nameof(GetImage), new { userId }, new {}),
            _ => BadRequest()
        };
    }

    [HttpGet(Name = nameof(GetImage))]
    public async Task<IActionResult> GetImage([FromRoute] Guid userId)
    {
        var result = await _storageService.GetImageAsync(userId);
        if (result.responseStream is not {})
        {
            return NotFound();
        }

        return File(result.responseStream, result.contentType);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteImage([FromRoute] Guid userId)
    {
        var result = await _storageService.RemoveImageAsync(userId);

        return result switch
        {
            true => NoContent(),
            _ => BadRequest()
        };
    }
}