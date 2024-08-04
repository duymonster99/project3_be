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
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly REDISCLIENT _redisclient;
        
        public UserController(ApplicationDbContext dbContext, IConfiguration configuration, REDISCLIENT client)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _redisclient = client;
        }
        
        [HttpPost("user/register")]
        public async Task<IActionResult> Register(UserDTO userDto)
        {
            try
            {
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
                if (existingUser != null)
                {
                    return BadRequest(new
                    {
                        Status = 404,
                        Message = "User already exists."
                    });
                }

                var user = new User
                {
                    FullName = userDto.FullName,
                    Email = userDto.Email,
                    Password = PasswordHelper.HashPassword(userDto.Password),
                    Role = "User"
                };

                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();

                var token = JwtHelper.GenerateToken(user.Email, _configuration["Jwt:Key"], "User");

                //_redisclient.Publish("user_register", $"{user.FullName} | {user.Email}");

                return Ok(new
                {
                    Status = 200,
                    Message = "User registered successfully.",
                    Token = token
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = 500,
                    Message = e.Message
                });
            }
        }
        
        [HttpPost("admin/register")]
        public async Task<IActionResult> AdminRegister(UserDTO userDto)
        {
            try
            {
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
                if (existingUser != null)
                {
                    return BadRequest(new
                    {
                        Status = 404,
                        Message = "User already exists."
                    });
                }

                var user = new User
                {
                    FullName = userDto.FullName,
                    Email = userDto.Email,
                    Password = PasswordHelper.HashPassword(userDto.Password),
                    Role = "Admin"
                };

                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();

                var token = JwtHelper.GenerateToken(user.Email, _configuration["Jwt:Key"], "Admin");

                //_redisclient.Publish("user_register", $"{user.FullName} | {user.Email}");

                return Ok(new
                {
                    Status = 200,
                    Message = "Admin registered successfully.",
                    Token = token
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = 500,
                    Message = e.Message
                });
            }
        }

        [HttpPost("user/login")]
        public async Task<IActionResult> UserLogin(LoginDTO loginDto)
        {
            try
            {
                var existUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Identifier);
                if (existUser == null)
                {
                    return BadRequest(new
                    {
                        Status = 404,
                        Message = "User not found"
                    });
                }

                if (!PasswordHelper.VerifyPassword(loginDto.Password, existUser.Password))
                {
                    return BadRequest(new
                    {
                        Status = 404,
                        Message = "Password is incorrect."
                    });
                }

                var token = JwtHelper.GenerateToken(existUser.Id.ToString(), _configuration["Jwt:Key"], existUser.Role);
                existUser.RefreshToken = Guid.NewGuid().ToString();
                existUser.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(2);
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    Status = 200,
                    Data = existUser,
                    Token = token,
                    RefreshToken = existUser.RefreshToken
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = 500,
                    Message = "Server error, please try again later.",
                    Details = e.Message
                });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenRefresh tokenRefresh)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == tokenRefresh.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest(new
                {
                    Status = 404,
                    Message = "Invalid refresh token."
                });
            }
            var tokenString = JwtHelper.GenerateToken(user.Email, _configuration["Jwt:Key"], user.Role);
            user.RefreshToken = Guid.NewGuid().ToString();
            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(2);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                Status = 200,
                Token = tokenString,
                RefreshToken = user.RefreshToken
            });
        }
        
        [HttpGet("test-admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult TestAdmin()
        {
            try
            {
                return Ok(new
                {
                    Status = 200,
                    Message = "You are an Admin."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = 500,
                    Message = ex.Message
                });
            }
        }
    }
}
