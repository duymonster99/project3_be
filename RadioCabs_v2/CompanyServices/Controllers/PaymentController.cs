using CompanyServices.Database;
using CompanyServices.DTOs;
using CompanyServices.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedisClient;

namespace CompanyServices.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly CompanyDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly REDISCLIENT _redisclient;

        public PaymentController(CompanyDbContext dbContext, IConfiguration configuration, REDISCLIENT redisclient)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _redisclient = redisclient;
        }

        [HttpPost("company/payment")]
        public async Task<IActionResult> AddPayment(PaymentDto paymentDto)
        {
            try
            {

                var payment = new Payment
                {
                    CompanyId = paymentDto.Id,
                    Amount = paymentDto.Amount,
                    ContentPayment = paymentDto.ContentPayment,
                    PaymentAt = paymentDto.PaymentAt,
                    PaymentTerm = paymentDto.PaymentTerm,
                    IsPayment = true,
                };

                await _dbContext.Payments.AddAsync(payment);
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    Status = 200,
                    Message = "Payment registered successfully"
                });

            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    Status = 404,
                    Message = "Company REGISTER request ERROR at CompanyAuthController - /api/v1/company/register",
                    Error = e.Message,
                    InnerError = e.InnerException?.Message
                });
            }

        }
    }
}
