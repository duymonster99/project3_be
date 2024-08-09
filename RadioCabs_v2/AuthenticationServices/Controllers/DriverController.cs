using AuthenticationServices.Database;
using AuthenticationServices.DTOs;
using AuthenticationServices.Helper;
using AuthenticationServices.Models;
using CompanyServices.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedisClient;

namespace AuthenticationServices.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly REDISCLIENT _redisclient;
        
        public DriverController(ApplicationDbContext dbContext, IConfiguration configuration, REDISCLIENT client)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _redisclient = client;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(DriverDto driverDto)
        {
            try
            {
                var existingDriver = await _dbContext.Drivers.Include(d => d.DriverInfo).FirstOrDefaultAsync(d => d.DriverMobile == CheckingPattern.AddPrefixMobile(driverDto.DriverMobile));
                if (existingDriver != null)
                {
                    return BadRequest(new
                    {
                        Status = 404,
                        Message = "Driver already exists."
                    });
                }

                var driverCodeRandom = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
                var driver = new Driver
                {
                    DriverFullName = driverDto.DriverFullName,
                    DriverCode = driverCodeRandom,
                    DriverMobile = CheckingPattern.AddPrefixMobile(driverDto.DriverMobile),
                    Password = PasswordHelper.HashPassword(driverDto.Password),
                    CompanyId = driverDto.CompanyId,
                    IsActive = false,
                    Rating = 4,
                    IsOnline = false,
                    Role = "Driver"
                };

                await _dbContext.Drivers.AddAsync(driver);
                await _dbContext.SaveChangesAsync();

                var token = JwtHelper.GenerateToken(driver.DriverMobile, _configuration["Jwt:Key"], "Driver");

                //_redisclient.Publish("driver_register",
                //    $"{driver.DriverFullName} | {driver.DriverMobile} | {driver.DriverCode}");

                return Ok(new
                {
                    Message = "Driver registered successfully",
                    Driver = driver,
                    Token = token
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An error for REQUEST - method POST - api/Driver/register - DriverController",
                    Error = e.Message,
                    ErrorStack = e.StackTrace
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            try
            {
                var drivers = await _dbContext.Drivers.ToListAsync();
                var existingDriver = drivers.FirstOrDefault(d => CheckingPattern.RemovePrefixMobile(d.DriverMobile) == loginDto.Identifier);
                if (existingDriver == null || !PasswordHelper.VerifyPassword(loginDto.Password, existingDriver.Password))
                {
                    return BadRequest(new
                    {
                        StatusCode = 404,
                        Message = "Driver not found or password is incorrect."
                    });
                }

                var token = JwtHelper.GenerateToken(existingDriver.Id.ToString(), _configuration["Jwt:Key"]!, "Driver");
                existingDriver.RefreshToken = Guid.NewGuid().ToString();
                existingDriver.RefreshTokenExpiryTime = DateTime.Now.AddSeconds(60);
                
                await _dbContext.SaveChangesAsync();
                
                return Ok(new
                {
                    Message = "Driver logged in successfully",
                    Driver = existingDriver,
                    Token = token,
                    RefreshToken = existingDriver.RefreshToken
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

        [HttpGet("company/{companyId}/drivers")]
        public async Task<IActionResult> GetDriversByCompanyId(int companyId)
        {
            try
            {
                var drivers = await _dbContext.Drivers.Where(d => d.CompanyId == companyId).Include(d => d.FeedbackDrivers).ToListAsync();
                if (drivers == null || !drivers.Any())
                {
                    return NotFound(new
                    {
                        Status = 404,
                        Message = "No drivers found for this company."
                    });
                }

                return Ok(new { Status = 200, Message = "Success", Drivers = drivers });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(new { Status = 400, Message = "Error: " + e.Message });
            }
        }

        [HttpPut("driver/{id}/update")]
        public async Task<IActionResult> UpdateDriver(int id, [FromForm] IFormFile? formFileProfile, IFormFile? formFileLicense)
        {
            var driver = await _dbContext.Drivers.Include(d => d.DriverInfo).FirstOrDefaultAsync(d => d.Id == id);
            if (driver == null)
            {
                return NotFound(new
                {
                    Status = 404,
                    Message = "Driver not found"
                });
            }

            // Ensure DriverInfo is not null
            if (driver.DriverInfo == null)
            {
                driver.DriverInfo = new DriverInfo
                {
                    DriverId = driver.Id
                };
                _dbContext.DriverInfos.Add(driver.DriverInfo);
            }
            
            if (formFileProfile != null && formFileProfile.Length > 0)
            {
                var imageUrl = await FileUpload.SaveImageAsync("DriverProfileAvatars", formFileProfile);
                driver.DriverInfo.DriverPersonalImage = imageUrl;
            }

            if (formFileLicense != null && formFileLicense.Length > 0)
            {
                var imageUrl = await FileUpload.SaveImageAsync("DriverProfileLicense", formFileLicense);
                driver.DriverInfo.DriverLicenseImage = imageUrl;
            }

            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok(new
                {
                    Status = 200,
                    Message = "Driver updated successfully"
                });
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (!_dbContext.Drivers.Any(d => d.Id == id))
                {
                    return NotFound(new
                    {
                        Status = 404,
                        Message = "Driver not found - api/v1/admin/updateDriver - AdminController/AuthenticationService"
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = 500,
                    Message = "Error! Can't update data from api/v1/admin/updateDriver - AdminController/AuthenticationServices"
                });
            }
        }

        [HttpPost("driver/feedback/create")]
        public async Task<IActionResult> CreateFeedbackDriver(FeedbackDriverDto feedbackDriverDto)
        {
            try
            {
                var feedbackDriver = new FeedbackDriver
                {
                    CompanyId = feedbackDriverDto.CompanyId,
                    DriverId = feedbackDriverDto.DriverId,
                    Name = feedbackDriverDto.Name,
                    Email = feedbackDriverDto.Email,
                    Description = feedbackDriverDto.Description,
                    Rating = feedbackDriverDto.Rating,
                };

                await _dbContext.FeedbackDrivers.AddAsync(feedbackDriver);
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Successfully"
                });
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

        [HttpGet("driver/{companyId}/feedback")]
        public async Task<IActionResult> GetFeedbackDriverByCompanyId(int companyId)
        {
            try
            {
                var drivers = await _dbContext.FeedbackDrivers.Where(f => f.CompanyId == companyId).ToListAsync();
                if (drivers == null || !drivers.Any())
                {
                    return NotFound(new
                    {
                        Status = 404,
                        Message = "No drivers found for this company."
                    });
                }

                return Ok(new { Status = 200, Message = "Success", Drivers = drivers });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = 400, Message = "Error: " + ex.Message });
            }
        }

        [HttpPost("driver/create/booking")]
        public async Task<IActionResult> CreateBooking (BookingDto bookingDto)
        {
            try
            {
                var booking = new Booking
                {
                    Name = bookingDto.Name,
                    Mobile = bookingDto.Mobile,
                    FromCity = bookingDto.FromCity,
                    FromDistrict = bookingDto.FromDistrict,
                    FromWard = bookingDto.FromWard,
                    FromAddress = bookingDto.FromAddress,
                    ToCity = bookingDto.ToCity,
                    ToDistrict = bookingDto.ToDistrict,
                    ToWard = bookingDto.ToWard,
                    ToAddress = bookingDto.ToAddress,
                    IsReceive = false,
                    IsNew = true,
                    DriverId = bookingDto.DriverId,
                };
                await _dbContext.Bookings.AddAsync(booking);
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Successfully"
                });
            }
            catch (Exception ex) 
            {
                return BadRequest(new { Status = 400, Message = "Error: " + ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenRefresh refreshTokenDto)
        {
            var driver = await _dbContext.Drivers.FirstOrDefaultAsync(u => u.RefreshToken == refreshTokenDto.RefreshToken);
            if (driver == null || driver.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized(new
                {
                    Message = "Invalid refresh token"
                });
            }
            
            var tokenString = JwtHelper.GenerateToken(driver.Id.ToString(), _configuration["Jwt:Key"]!, "Driver");
            driver.RefreshToken = Guid.NewGuid().ToString();
            driver.RefreshTokenExpiryTime = DateTime.Now.AddSeconds(60);
            await _dbContext.SaveChangesAsync();
            
            return Ok(new
            {
                StatusCode = 200,
                Message = "Token refreshed successfully",
                Token = tokenString,
                RefreshToken = driver.RefreshToken
            });
        }
        
        // Test AUTHORIZE DRIVER
        [HttpGet("authorize")]
        [Authorize(Roles = "Driver")]
        public IActionResult TestAuthorize()
        {
            try
            {
                return Ok(new
                {
                    Message = "Driver authorized successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An error for REQUEST - method GET - api/Driver/authorize - DriverController",
                    Error = ex.Message,
                    ErrorStack = ex.StackTrace
                });
            }
        }
    }
}
