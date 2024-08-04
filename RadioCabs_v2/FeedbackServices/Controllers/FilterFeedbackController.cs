using FeedbackServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RedisClient;
using StackExchange.Redis;

namespace FeedbackServices.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FilterFeedbackController : ControllerBase
{
    private readonly FeedbackRepository _dbContext;
    private readonly IConfiguration _configuration;
    private readonly REDISCLIENT _redisclient;
        
        
    public FilterFeedbackController(FeedbackRepository dbContext, IConfiguration configuration, REDISCLIENT redisclient)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _redisclient = redisclient;
    }
    
    // Test AUTHORIZE ADMIN
    [HttpGet("testAuthorize")]
    [Authorize(Roles = "Admin")]
    public IActionResult TestAuthorize()
    {
        try
        {
            return Ok(new
            {
                StatusCode = 200,
                Message = "You are authorized"
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    // TOP 10 new Feedback
    [HttpGet("GetTop10Feedback")]
    public async Task<IActionResult> GetTop10Feedback()
    {
        try
        {
            var feedbacks = await _dbContext.GetAllAsync();
            var top10Feedbacks = feedbacks.OrderByDescending(f => f.CreatedAt).Take(10).ToList();
            return Ok(top10Feedbacks);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
    
    // GET MOST FEEDBACK BY CUSTOMER EMAIL
    // [HttpGet("GetMostFeedbackByEmail")]
    // public async Task<IActionResult> GetMostFeedbackByEmail(string emailPart)
    // {
    //     try
    //     {
    //         var feedbacks = await _dbContext.GetByCustomerEmailAsync(emailPart);
    //         var mostFeedback = feedbacks.GroupBy(f => f.Email).OrderByDescending(g => g.Count()).FirstOrDefault();
    //         return Ok(mostFeedback);
    //     }
    //     catch (Exception e)
    //     {
    //         return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
    //     }
    // }
}