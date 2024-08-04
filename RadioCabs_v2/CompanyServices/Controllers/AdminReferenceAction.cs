using CompanyServices.Database;
using CompanyServices.DTOs;
using CompanyServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedisClient;

namespace CompanyServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminReferenceAction : ControllerBase
    {
        private readonly CompanyDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly REDISCLIENT _redisclient;

        public AdminReferenceAction(CompanyDbContext dbContext, IConfiguration configuration, REDISCLIENT redisclient)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _redisclient = redisclient;
        }

        // GET ALL COMPANIES
        [HttpGet("allCompaniesInfo")]
        public async Task<IActionResult> GetAllCompanies()
        {
            try
            {
                var companies = await _dbContext.Companies
                    .Include(c => c.CompanyLocationServices)
                    .Include(c => c.CompanyServices)
                    .Include(c => c.Payments)
                    .ToListAsync();

                if (companies == null || companies.Count == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No Companies Found"
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Companies retrieved successfully",
                    Data = companies
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "There was an error retrieving the companies",
                    Detail = ex.Message
                });
            }
        }

        // DELETE COMPANY AND ALL RELATED INFORMATION
        [HttpDelete("company/{id}/delete")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                var company = await _dbContext.Companies
                    .Include(c => c.CompanyLocationServices)
                    .Include(c => c.CompanyServices)
                    .Include(c => c.Advertisements)
                    .Include(c => c.Payments)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (company == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Company not found"
                    });
                }

                _dbContext.Companies.Remove(company);
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Company deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An error occurred while deleting the company",
                    Error = ex.Message
                });
            }
        }

        // GET COMPANY BY ID
        [HttpGet("company/{id}")]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            try
            {
                var company = await _dbContext.Companies
                    .Include(c => c.CompanyLocationServices)
                    .Include(c => c.CompanyServices)
                    .Include(c => c.Advertisements)
                    .Include(c => c.Payments)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (company == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Company not found"
                    });
                }

                return Ok(company);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "There was an error retrieving the company",
                    Detail = ex.Message
                });
            }
        }

        // GET COMPANY BY TAX CODE
        [HttpGet("company/taxCode/{taxCode}")]
        public async Task<IActionResult> GetCompanyByTaxCode(string taxCode)
        {
            try
            {
                var company = await _dbContext.Companies
                    .Include(c => c.CompanyLocationServices)
                    .Include(c => c.CompanyServices)
                    .Include(c => c.Advertisements)
                    .Include(c => c.Payments)
                    .FirstOrDefaultAsync(c => c.CompanyTaxCode == taxCode);

                if (company == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Company not found"
                    });
                }

                return Ok(company);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "There was an error retrieving the company",
                    Detail = ex.Message
                });
            }
        }

        // GET COMPANY BY NAME
        [HttpGet("company/name/{name}")]
        public async Task<IActionResult> GetCompanyByName(string name)
        {
            try
            {
                var companies = await _dbContext.Companies
                    .Include(c => c.CompanyLocationServices)
                    .Include(c => c.CompanyServices)
                    .Include(c => c.Advertisements)
                    .Where(c => c.CompanyName.Contains(name))
                    .ToListAsync();

                if (companies == null || companies.Count == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No companies found with the given name"
                    });
                }

                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "There was an error retrieving the companies",
                    Detail = ex.Message
                });
            }
        }

        // GET COMPANY BY CITY
        [HttpGet("company/city/{city}")]
        public async Task<IActionResult> GetCompanyByCity(string city)
        {
            try
            {
                var companies = await _dbContext.Companies
                    .Include(c => c.CompanyLocationServices)
                    .Include(c => c.CompanyServices)
                    .Include(c => c.Advertisements)
                    .Include(c => c.Payments)
                    .Where(c => c.CompanyCity.Contains(city))
                    .ToListAsync();

                if (companies == null || companies.Count == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "No companies found in the given city"
                    });
                }

                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "There was an error retrieving the companies",
                    Detail = ex.Message
                });
            }
        }

        // TEST AUTHORIZE JWT ROLE = ADMIN
        [HttpGet("testAuthorize")]
        [Authorize(Roles = "Admin")]
        public IActionResult TestAuthorize()
        {
            return Ok(new
            {
                StatusCode = 200,
                Message = "You are authorized"
            });
        }
    }
}
