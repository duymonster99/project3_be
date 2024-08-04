using CompanyServices.DTOs;
using CompanyServices.Database;
using CompanyServices.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RedisClient;
using CompanyServices.Helper;


namespace CompanyServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementImageController : ControllerBase
    {
        private readonly CompanyDbContext _dbContext;
        private readonly REDISCLIENT _redisclient;

        public AdvertisementImageController(CompanyDbContext dbContext, REDISCLIENT redisclient)
        {
            _dbContext = dbContext;
            _redisclient = redisclient;
        }

        // Create new advertisement image
        [HttpPost("create")]
        public async Task<IActionResult> CreateAdvertisementImage([FromForm] AdvertisementImageDto imageDto, IFormFile formFile)
        {
            try
            {
                var company = await _dbContext.Companies.FindAsync(imageDto.CompanyId);
                if (company == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Company not found"
                    });
                }

                var imageUrl = await FileUpload.SaveImageAsync("AdvertisementImages", formFile);

                var advertisementImage = new AdvertisementImage
                {
                    CompanyId = imageDto.CompanyId,
                    ImageUrl = imageUrl,
                    Description = imageDto.Description
                };

                await _dbContext.AdvertisementImages.AddAsync(advertisementImage);
                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAdvertisementImageById), new { id = advertisementImage.Id }, advertisementImage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while creating the advertisement image",
                    Error = ex.Message
                });
            }
        }

        // Get advertisement image by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdvertisementImageById(int id)
        {
            try
            {
                var advertisementImage = await _dbContext.AdvertisementImages.FindAsync(id);
                if (advertisementImage == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Advertisement image not found"
                    });
                }

                return Ok(advertisementImage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching the advertisement image",
                    Error = ex.Message
                });
            }
        }

        // Get all advertisement images of a company
        [HttpGet("company/{companyId}/images")]
        public async Task<IActionResult> GetAdvertisementImagesByCompanyId(int companyId)
        {
            try
            {
                var images = await _dbContext.AdvertisementImages.Where(ai => ai.CompanyId == companyId).ToListAsync();
                if (images == null || images.Count == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No advertisement images found for this company"
                    });
                }

                return Ok(images);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching the advertisement images",
                    Error = ex.Message
                });
            }
        }

        // Get advertisement images by type (description)
        [HttpGet("company/{companyId}/images/by-type")]
        public async Task<IActionResult> GetAdvertisementImagesByType(int companyId, string type)
        {
            try
            {
                var images = await _dbContext.AdvertisementImages.Where(ai => ai.CompanyId == companyId && ai.Description == type).ToListAsync();
                if (images == null || images.Count == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No advertisement images found for this company with the specified type"
                    });
                }

                return Ok(images);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching the advertisement images by type",
                    Error = ex.Message
                });
            }
        }

        // Update advertisement image
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAdvertisementImage(int id, [FromForm] AdvertisementImageDto imageDto)
        {
            try
            {
                var advertisementImage = await _dbContext.AdvertisementImages.FindAsync(id);
                if (advertisementImage == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Advertisement image not found"
                    });
                }

                if (imageDto.ImageFile != null && imageDto.ImageFile.Length > 0)
                {
                    var imageUrl = await UploadImageAsync(imageDto.ImageFile);
                    advertisementImage.ImageUrl = imageUrl;
                }

                if (!string.IsNullOrEmpty(imageDto.Description))
                {
                    advertisementImage.Description = imageDto.Description;
                }

                _dbContext.AdvertisementImages.Update(advertisementImage);
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Advertisement image updated successfully",
                    Data = advertisementImage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while updating the advertisement image",
                    Error = ex.Message
                });
            }
        }

        // Delete advertisement image by ID
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAdvertisementImage(int id)
        {
            try
            {
                var advertisementImage = await _dbContext.AdvertisementImages.FindAsync(id);
                if (advertisementImage == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Advertisement image not found"
                    });
                }

                _dbContext.AdvertisementImages.Remove(advertisementImage);
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Advertisement image deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while deleting the advertisement image",
                    Error = ex.Message
                });
            }
        }

        // Delete multiple advertisement images by IDs
        [HttpDelete("delete-multiple")]
        public async Task<IActionResult> DeleteMultipleAdvertisementImages([FromBody] List<int> ids)
        {
            try
            {
                var advertisementImages = await _dbContext.AdvertisementImages.Where(ai => ids.Contains(ai.Id)).ToListAsync();
                if (advertisementImages == null || advertisementImages.Count == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No advertisement images found with the specified IDs"
                    });
                }

                _dbContext.AdvertisementImages.RemoveRange(advertisementImages);
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Advertisement images deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while deleting the advertisement images",
                    Error = ex.Message
                });
            }
        }

        // Helper method to upload image and return URL
        private async Task<string> UploadImageAsync(IFormFile file)
        {
            await Task.Delay(500); // Simulate async work
            return "https://placeholder.com/your-uploaded-image-url";
        }
    }
}
