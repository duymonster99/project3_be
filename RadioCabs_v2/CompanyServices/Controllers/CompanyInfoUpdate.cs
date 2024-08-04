using System.Security.Cryptography.Pkcs;
using CompanyServices.Database;
using CompanyServices.DTOs;
using CompanyServices.DTOs;
using CompanyServices.Helper;
using CompanyServices.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedisClient;

namespace CompanyServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyInfoUpdate : ControllerBase
    {
        private readonly CompanyDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly REDISCLIENT _redisclient;
        
        
        public CompanyInfoUpdate(CompanyDbContext dbContext, IConfiguration configuration, REDISCLIENT redisclient)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _redisclient = redisclient;
        }
        
        // CRUD 
        // Update company info
        [HttpPut("company/{id}/update")]
        public async Task<IActionResult> UpdateCompanyInfo(int id, [FromBody] CompanyInfoDto companyInfoDto)
        {
            try
            {
                var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Id == id);
                if (company == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Company not found"
                    });
                }
                
                if (!string.IsNullOrEmpty(companyInfoDto.CompanyName))
                {
                    company.CompanyName = companyInfoDto.CompanyName;
                }

                if (!string.IsNullOrEmpty(companyInfoDto.CompanyEmail))
                {
                    company.CompanyEmail = companyInfoDto.CompanyEmail;
                }
                
                // if (!string.IsNullOrEmpty(companyInfoDto.CompanyPassword))
                // {
                //     company.CompanyPassword = companyInfoDto.CompanyPassword;
                // }
                
                if (!string.IsNullOrEmpty(companyInfoDto.ContactPersonName))
                {
                    company.ContactPerson = companyInfoDto.ContactPersonName;
                }

                if (!string.IsNullOrEmpty(companyInfoDto.Designation))
                {
                    company.Designation = companyInfoDto.Designation;
                }

                if (!string.IsNullOrEmpty(companyInfoDto.ContactPersonMobile))
                {
                    company.ContactPersonMobile = companyInfoDto.ContactPersonMobile;
                }

                if (!string.IsNullOrEmpty(companyInfoDto.CompanyTelephone))
                {
                    company.CompanyTelephone = companyInfoDto.CompanyTelephone;
                }

                if (!string.IsNullOrEmpty(companyInfoDto.CompanyAddress))
                {
                    company.CompanyAddress = companyInfoDto.CompanyAddress;
                }

                if (!string.IsNullOrEmpty(companyInfoDto.CompanyWard))
                {
                    company.CompanyWard = companyInfoDto.CompanyWard;
                }

                if (!string.IsNullOrEmpty(companyInfoDto.CompanyDistrict))
                {
                    company.CompanyDistrict = companyInfoDto.CompanyDistrict;
                }

                if (!string.IsNullOrEmpty(companyInfoDto.CompanyCity))
                {
                    company.CompanyCity = companyInfoDto.CompanyCity;
                }

                if (!string.IsNullOrEmpty(companyInfoDto.MembershipType))
                {
                    company.MembershipType = companyInfoDto.MembershipType;
                }

                if (!string.IsNullOrEmpty(companyInfoDto.IsActive.ToString())) //
                {
                    company.IsActive = companyInfoDto.IsActive;
                }

                // Cập nhật hình ảnh của công ty
                //if (formFile != null && formFile.Length > 0)
                //{
                //    var imagePath = await FileUpload.SaveImageAsync("CompanyImageProfile", formFile);
                //    company.CompanyImageUrl = imagePath;
                //}

                //if (companyInfoDto.IsActive != null)
                //{
                //    company.IsActive = companyInfoDto.IsActive;
                //}

                // if (companyInfoDto.CompanyImage != null && companyInfoDto.CompanyImage.Length > 0)
                // {
                //     // Assuming you have a method to upload the image and get the URL
                //     var contentType = BlobContentTypes.GetContentType(companyInfoDto.CompanyImage);
                //     company.CompanyImageUrl = await _blobServices.UploadBlobWithContentTypeAsync(companyInfoDto.CompanyImage, contentType);
                // }

                _dbContext.Companies.Update(company);
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Company updated successfully",
                    Data = company
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while updating the company",
                    Error = ex.Message
                });
            }
        }
        
    }
}
