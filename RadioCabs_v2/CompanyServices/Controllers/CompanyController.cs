using CompanyServices.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedisClient;

namespace CompanyServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly REDISCLIENT _redisclient;
        private readonly HttpClient _httpClient;
        // Default URL
        private readonly string _defaultUrl = "http://localhost:5192";

        public CompanyController(CompanyDbContext dbContext, IConfiguration configuration, REDISCLIENT redisclient, HttpClient httpClient)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _redisclient = redisclient;
            _httpClient = httpClient;
        }

        [HttpGet("{companyId}/drivers")]
        public async Task<IActionResult> GetDriversOfCompany(int companyId)
        {
            // /api/DriverCompany/company/1/drivers
            var url = $"{_defaultUrl}/api/DriverCompany/company/{companyId}/drivers";
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                }
                var drivers = await response.Content.ReadAsStringAsync();

                // var drivers = DeserializeObject(await response.Content.ReadAsStringAsync());

                return Ok(new
                {
                    Status = 200,
                    Message = "Success",
                    Drivers = drivers
                });
            }
            catch (Exception e)
            {
                return BadRequest(new { Status = 400, Message = "Error: " + e.Message });
            }
        }
    }
}
