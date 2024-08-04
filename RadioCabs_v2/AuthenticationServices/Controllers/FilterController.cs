using AuthenticationServices.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedisClient;

namespace AuthenticationServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilterController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly REDISCLIENT _redisclient;
        
        public FilterController(ApplicationDbContext dbContext, IConfiguration configuration, REDISCLIENT client)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _redisclient = client;
        }

        [HttpGet("driver/city/{city}")]
        public async Task<IActionResult> FilterDriverByCity(string city)
        {
            try
            {
                var drivers = await _dbContext.Drivers.Include(d => d.DriverInfo).Where(d => d.DriverInfo.City == city).ToListAsync();
                if (drivers.Count == 0)
                {
                    return NotFound(new
                    {
                        Status = 404,
                        Message = "No driver found in this city."
                    });
                }
                return Ok(new
                {
                    Status = 200,
                    Message = "Drivers found.",
                    Data = drivers
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
    }
}