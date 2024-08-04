using CompanyServices.DTOs;
using AuthenticationServices.Helper;
using CompanyServices.Database;
using CompanyServices.DTOs;
using CompanyServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedisClient;

namespace CompanyServices.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CompanyAuthController : ControllerBase
    {
        private readonly CompanyDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly REDISCLIENT _redisclient;


        public CompanyAuthController(CompanyDbContext dbContext, IConfiguration configuration, REDISCLIENT redisclient)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _redisclient = redisclient;
        }

        [HttpPost("company/register")]
        public async Task<IActionResult> Register(CompanyDto companyDto)
        {
            try
            {
                var existingCompany =
                    await _dbContext.Companies.FirstOrDefaultAsync(c => c.CompanyTaxCode == companyDto.CompanyTaxCode);
                var existingCompanyName =
                    await _dbContext.Companies.FirstOrDefaultAsync(c => c.CompanyName == companyDto.CompanyName);
                if (existingCompany != null)
                {
                    return BadRequest(new
                    {
                        Status = 400,
                        Message = "Company already exists"
                    });
                }

                if (existingCompanyName != null)
                {
                    return BadRequest(new
                    {
                        Status = 400,
                        Message = "Name already exists"
                    });
                }

                var company = new Company
                {
                    CompanyName = companyDto.CompanyName,
                    CompanyTaxCode = companyDto.CompanyTaxCode,
                    CompanyEmail = companyDto.CompanyEmail,
                    CompanyPassword = PasswordHelper.HashPassword(companyDto.CompanyPassword),
                    MembershipType = companyDto.MembershipType, //
                    IsActive = false, //
                    Role = "Company"
                };

                await _dbContext.Companies.AddAsync(company);
                await _dbContext.SaveChangesAsync();

                var token = JwtHelper.GenerateToken(company.Id.ToString(), _configuration["Jwt:Key"], "Company");
                //_redisclient.Publish("company_register", $"{company.CompanyName} | {company.CompanyEmail}");

                return Ok(new
                {
                    Status = 200,
                    Message = "Company registered successfully",
                    data = company.Id //
                });

            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    Status = 404,
                    Message = "Company REGISTER request ERROR at CompanyAuthController - /api/v1/company/register",
                    Error = e.Message
                });
            }

        }

        [HttpPost("company/login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            try
            {
                var existCompany = await _dbContext.Companies.Include(c => c.Payments).FirstOrDefaultAsync(c => c.CompanyTaxCode == loginDto.Identifier);
                if (existCompany == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = 404,
                        Message = "Company not found"
                    });
                }

                if (!PasswordHelper.VerifyPassword(loginDto.Password, existCompany.CompanyPassword))
                {
                    return BadRequest(new
                    {
                        StatusCode = 404,
                        Message = "Password incorrect"
                    });
                }

                if (existCompany.Payments == null || existCompany.Payments.IsPayment == false)
                {
                    return BadRequest(new
                    {
                        StatusCode = 404,
                        Message = "Your account is unpaid. Please pay to access the admin page.",
                        Data = existCompany
                    });
                }

                var token = JwtHelper.GenerateToken(existCompany.Id.ToString(), _configuration["Jwt:Key"], "Company");
                existCompany.RefreshToken = Guid.NewGuid().ToString();
                existCompany.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(2);

                await _dbContext.SaveChangesAsync();
                return Ok(new
                {
                    Status = 200,
                    Message = "Company logged in successfully",
                    Company = existCompany,
                    Token = token,
                    RefreshToken = existCompany.RefreshToken
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An error for REQUEST - method POST - api/Driver/login - DriverController",
                    Error = ex.Message,
                    ErrorStack = ex.StackTrace
                });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenRefresh tokenRefresh)
        {
            var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.RefreshToken == tokenRefresh.RefreshToken);
            if (company == null || company.RefreshTokenExpiryTime < DateTime.Now)
            {
                return BadRequest(new
                {
                    Status = 404,
                    Message = "Invalid refresh token"
                });
            }

            var token = JwtHelper.GenerateToken(company.CompanyTaxCode, _configuration["Jwt:Key"], "Company");
            company.RefreshToken = Guid.NewGuid().ToString();
            company.RefreshTokenExpiryTime = DateTime.Now.AddSeconds(2);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                Status = 200,
                Message = "Token refreshed successfully",
                Token = token,
                RefreshToken = company.RefreshToken
            });
        }

        // TEST-AUTHORIZE
        [HttpGet("test-company")]
        [Authorize(Roles = "Company, Admin")]
        public IActionResult TestCompany()
        {
            try
            {
                return Ok(new
                {
                    Status = 200,
                    Message = "Company authorized"
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = 500,
                    Message = "An error for REQUEST - method GET - api/v1/company/test-company - CompanyAuthController",
                    Error = e.Message
                });
            }
        }

    }
}
