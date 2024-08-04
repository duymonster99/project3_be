using AuthenticationServices.Database;
using AuthenticationServices.DTOs;
using AuthenticationServices.Models;
using AuthenticationServices.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedisClient;

namespace AuthenticationServices.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly REDISCLIENT _redisClient;
        private readonly EmailServices emailServices;

        public PasswordController(ApplicationDbContext dbContext, IConfiguration configuration, REDISCLIENT client, EmailServices _emailServices)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _redisClient = client;
            emailServices = _emailServices;
        }

        // Forgot Password with OTP 
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == forgotPasswordDto.Email);
            if (user == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "User not found"
                });
            }

            // OTP 6 digit 
            var otp = new Random().Next(100000, 999999).ToString();
            //_redisClient.Set($"otp_{forgotPasswordDto.Email}", otp, TimeSpan.FromMinutes(5));

            // Check time if over 5 minutes , alert user to resend OTP
            //_redisClient.Publish("otp_event", $"{forgotPasswordDto.Email}|{otp}");
            return Ok(new
            {
                StatusCode = 200,
                Message = "OTP sent to your email",
                Data = otp,
                users = user.FullName
            });
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendMail(EmailRequest emailRequest)
        {
            if (string.IsNullOrEmpty(emailRequest.Subject)
                || string.IsNullOrEmpty(emailRequest.ToMail)
                || string.IsNullOrEmpty(emailRequest.HtmlContent))
            {
                return BadRequest("Email detail are not complete");
            }

            try
            {
                await emailServices.SendEmailAsync(emailRequest);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "OTP sent to your email",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            //var otp = _redisClient.Get($"otp_{resetPasswordDto.Email}");
            //if (otp == null || otp != resetPasswordDto.OTP)
            //{
            //    return BadRequest(new { Status = 400, Message = "Invalid OTP" });
            //}

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == resetPasswordDto.Email);
            if (user == null)
            {
                return NotFound(new { Status = 404, Message = "User not found" });
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            //_redisClient.Remove($"otp_{resetPasswordDto.Email}");

            return Ok(new { Status = 200, Message = "Password reset successfully" });
        }
    }
}
