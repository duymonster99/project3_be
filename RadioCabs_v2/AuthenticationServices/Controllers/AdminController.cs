using AuthenticationServices.Database;
using AuthenticationServices.DTOs;
using AuthenticationServices.Helper;
using AuthenticationServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedisClient;

namespace AuthenticationServices.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly REDISCLIENT _redisclient;
        
        public AdminController(ApplicationDbContext dbContext, IConfiguration configuration, REDISCLIENT client)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _redisclient = client;
        }

        // CRUD Driver
        [HttpGet("getAllDrivers")]
        public async Task<IActionResult> GetAllDrivers()
        {
            try
            {
                var drivers = await _dbContext.Drivers.Include(d => d.DriverInfo).ToListAsync();
                return Ok(new
                {
                    Status = 200,
                    Data = drivers
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = 500,
                    Message = "Error! Can't get data from api/v1/admin/getAllDrivers - AdminController/AuthenticationServices"
                });
            }
        }

        [HttpGet("getDriverById/{id}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            try
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
                return Ok(new
                {
                    Status = 200,
                    Data = driver
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = 500,
                    Message = "Error! Can't get data from api/v1/admin/getDriverById - AdminController/AuthenticationServices"
                });
            }
        }
        
        [HttpPut("updateDriver/{id}")]
        public async Task<IActionResult> UpdateDriver(int id, [FromBody] DriverInfoDto driverInfoDto)
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
            
            // PASSWORD
            if (!string.IsNullOrEmpty(driverInfoDto.Password))
            {
                driver.Password = BCrypt.Net.BCrypt.HashPassword(driverInfoDto.Password);
            }
            
            // STATUS BOOLEAN
            if (!string.IsNullOrEmpty(driverInfoDto.IsActive.ToString()))
            {
                driver.IsActive = driverInfoDto.IsActive;
            }
            // DRIVER MOBILE 
            if (!string.IsNullOrEmpty(driverInfoDto.DriverMobile))
            {
                driver.DriverMobile = driverInfoDto.DriverMobile;
            }
            
            // DRIVER CODE 
            if (!string.IsNullOrEmpty(driverInfoDto.DriverCode))
            {
                driver.DriverCode = driverInfoDto.DriverCode;
            }

            if (!string.IsNullOrEmpty(driverInfoDto.DriverFullName))
            {
                driver.DriverFullName = driverInfoDto.DriverFullName;
            }

            if (!string.IsNullOrEmpty(driverInfoDto.DriverEmail))
            {
                driver.DriverEmail = driverInfoDto.DriverEmail;
            }

            if (!string.IsNullOrEmpty(driverInfoDto.RegistrationCar))
            {
                driver.DriverInfo.RegistrationCar = driverInfoDto.RegistrationCar;
            }

            if (!string.IsNullOrEmpty(driverInfoDto.DriverLicense))
            {
                driver.DriverInfo.DriverLicense = driverInfoDto.DriverLicense;
            }

            if (!string.IsNullOrEmpty(driverInfoDto.Address))
            {
                driver.DriverInfo.Address = driverInfoDto.Address;
            }

            if (!string.IsNullOrEmpty(driverInfoDto.Street))
            {
                driver.DriverInfo.Street = driverInfoDto.Street;
            }

            if (!string.IsNullOrEmpty(driverInfoDto.Ward))
            {
                driver.DriverInfo.Ward = driverInfoDto.Ward;
            }

            if (!string.IsNullOrEmpty(driverInfoDto.City))
            {
                driver.DriverInfo.City = driverInfoDto.City;
            }

            // if (driverInfoDto.DriverPersonalImage != null)
            // {
            //     var contentType = BlobContentTypes.GetContentType(driverInfoDto.DriverPersonalImage);
            //     driver.DriverInfo.DriverPersonalImage = await _blobServices.UploadBlobWithContentTypeAsync(driverInfoDto.DriverPersonalImage, contentType);
            // }

            // if (driverInfoDto.DriverLicenseImage != null)
            // {
            //     var contentType = BlobContentTypes.GetContentType(driverInfoDto.DriverLicenseImage);
            //     driver.DriverInfo.DriverLicenseImage = await _blobServices.UploadBlobWithContentTypeAsync(driverInfoDto.DriverLicenseImage, contentType);
            // }

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

        
        [HttpDelete("deleteDriver/{id}")]
        public async Task<IActionResult> DeleteDriver(int id)
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
            try
            {
                _dbContext.Drivers.Remove(driver);
                await _dbContext.SaveChangesAsync();
                return Ok(new
                {
                    Status = 200,
                    Message = "Driver deleted successfully"
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = 500,
                    Message = "Error! Can't delete data from api/v1/admin/deleteDriver - AdminController/AuthenticationServices"
                });
            }
        }
        
        // CRUD User 
        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                // Query the Users table in the database context
                // Filter records where the Role is "User"
                // Include the related UserInfo data for each user
                var users = await _dbContext.Users
                    .Where(u => u.Role == "User") 
                    .Include(u => u.UserInfo)
                    .ToListAsync();

                // Return an OK status code with the list of users
                return Ok(new
                {
                    Status = 200,
                    Data = users
                });
            }
            catch (Exception e)
            {
                // If an exception occurs, return an Internal Server Error status code
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = 500,
                    Message = "Error! Can't get data from api/v1/admin/getAllUsers - AdminController/AuthenticationServices"
                });
            }
        }

        
        [HttpGet("getUserById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _dbContext.Users.Include(u => u.UserInfo).FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return NotFound(new
                    {
                        Status = 404,
                        Message = "User not found"
                    });
                }
                return Ok(new
                {
                    Status = 200,
                    Data = user
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = 500,
                    Message = "Error! Can't get data from api/v1/admin/getUserById - AdminController/AuthenticationServices"
                });
            }
        }

        [HttpPut("updateUser/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserInfoDTO userInfoDto)
        {
            var user = await _dbContext.Users.Include(u => u.UserInfo).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new
                {
                    Status = 404,
                    Message = "User not found"
                });
            }

            // Ensure UserInfo is not null
            if (user.UserInfo == null)
            {
                user.UserInfo = new UserInfo
                {
                    UserId = user.Id
                };
                _dbContext.UserInfos.Add(user.UserInfo);
            }

            if (!string.IsNullOrEmpty(userInfoDto.FullName))
            {
                user.FullName = userInfoDto.FullName;
            }

            if (!string.IsNullOrEmpty(userInfoDto.Mobile))
            {
                user.UserInfo.Mobile = userInfoDto.Mobile;
            }

            if (!string.IsNullOrEmpty(userInfoDto.Email))
            {
                user.Email = userInfoDto.Email;
            }
            
            if (!string.IsNullOrEmpty(userInfoDto.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(userInfoDto.Password);
            }
            
            if (!string.IsNullOrEmpty(userInfoDto.Role))
            {
                user.Role = userInfoDto.Role;
            }
            
            // Status Boolean
            if (!string.IsNullOrEmpty(userInfoDto.Status.ToString()))
            {
                user.Status = userInfoDto.Status;
            }

            if (!string.IsNullOrEmpty(userInfoDto.Address))
            {
                user.UserInfo.Address = userInfoDto.Address;
            }

            if (!string.IsNullOrEmpty(userInfoDto.Street))
            {
                user.UserInfo.Street = userInfoDto.Street;
            }

            if (!string.IsNullOrEmpty(userInfoDto.Ward))
            {
                user.UserInfo.Ward = userInfoDto.Ward;
            }

            if (!string.IsNullOrEmpty(userInfoDto.District))
            {
                user.UserInfo.District = userInfoDto.District;
            }

            if (!string.IsNullOrEmpty(userInfoDto.City))
            {
                user.UserInfo.City = userInfoDto.City;
            }
            
            if (!string.IsNullOrEmpty(userInfoDto.Location))
            {
                user.UserInfo.Location = userInfoDto.Location;
            }

            // if (userInfoDto.PersonalImage != null)
            // {
            //     var contentType = BlobContentTypes.GetContentType(userInfoDto.PersonalImage);
            //     user.UserInfo.Image = await _blobServices.UploadBlobWithContentTypeAsync(userInfoDto.PersonalImage, contentType);
            // }

            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok(new
                {
                    Status = 200,
                    Message = "User updated successfully"
                });
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (!_dbContext.Users.Any(u => u.Id == id))
                {
                    return NotFound(new
                    {
                        Status = 404,
                        Message = "User not found - api/v1/admin/updateUser - AdminController/AuthenticationServices"
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = 500,
                    Message = "Error! Can't update data from api/v1/admin/updateUser - AdminController/AuthenticationServices"
                });
            }
        }
        
        [HttpGet("getUserByRole/{role}")]
        public async Task<IActionResult> GetUserByRole(string role)
        {
            try
            {
                var users = await _dbContext.Users
                    .Where(u => u.Role == role)
                    .Include(u => u.UserInfo)
                    .ToListAsync();
                return Ok(new
                {
                    Status = 200,
                    Data = users
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = 500,
                    Message = "Error! Can't get data from api/v1/admin/getUserByRole - AdminController/AuthenticationServices"
                });
            }
        }

        
        [HttpDelete("deleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _dbContext.Users.Include(u => u.UserInfo).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new
                {
                    Status = 404,
                    Message = "User not found"
                });
            }
            try
            {
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
                return Ok(new
                {
                    Status = 200,
                    Message = "User deleted successfully"
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = 500,
                    Message = "Error! Can't delete data from api/v1/admin/deleteUser - AdminController/AuthenticationServices"
                });
            }
        }
        
        // Test Authorize Method
        [HttpGet("testAuthorize")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminAuthorize()
        {
            try
            {
                return Ok(new
                {
                    Message = "Admin is authorized to access this method"
                });
            }
            catch (Exception ex)
            {
                return Unauthorized( new
                {
                    Message = "Error by Server, check try block at Authorize method of AuthenticationService.DriverController",
                    Details = ex.Message
                });
            }
        }
    }
}
