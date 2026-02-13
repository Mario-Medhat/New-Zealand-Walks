using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }

        // POST: /api/Images/Upload
        [HttpPost]
        [Route("Upload")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto request)
        {
            ValidateImageUploadRequest(request);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // TODO: apply mapping from ImageUploadRequestDto to Image using AutoMapper

            // Convert DTO to domain model (this is where AutoMapper would typically be used)
            var imageDM = new Image
            {
                File = request.File,
                FileExtension = Path.GetExtension(request.File.FileName),
                FileSizeInBytes = request.File.Length,
                FileName = request.FileName,
                FileDescription = request.FileDescription,
            };

            // User repository to upload the image (this is where you would typically call a service or repository method to handle the file storage)
            await imageRepository.Upload(imageDM);
            return Ok(imageDM);
        }

        private void ValidateImageUploadRequest(ImageUploadRequestDto request)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var maxFileSizeInMB = 5; // 5MB
            var maxFileSize = maxFileSizeInMB * 1024 * 1024; // 5MB

            // Validate that the request is not null
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");

            // Validate file type based on extension
            if (!allowedExtensions.Contains(Path.GetExtension(request.File.FileName).ToLower()))
                ModelState.AddModelError("file", "Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.");

            // Validate file existence and size
            if (request.File == null || request.File.Length == 0)
                ModelState.AddModelError("file", "No file uploaded.");

            // Validate file size (e.g., max 5MB)
            if (request.File.Length > maxFileSize)
                ModelState.AddModelError("file", $"File size mpte than {maxFileSizeInMB}MB.");

            // Validate that the file name is provided
            if (string.IsNullOrWhiteSpace(request.FileName))
                throw new ArgumentException("File name is required.", nameof(request.FileName));
        }

    }
}
