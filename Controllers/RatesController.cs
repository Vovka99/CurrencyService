using CurrencyService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyService.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class RatesController : ControllerBase
{
    private readonly ICurrencyRateService _currencyRateService;

    public RatesController(ICurrencyRateService currencyRateService)
    {
        _currencyRateService = currencyRateService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCurrent([FromQuery] DateTime? date, CancellationToken cancellationToken)
    {
        date ??= DateTime.UtcNow.Date;

        if (date.Value.Date > DateTime.UtcNow.Date)
        {
            return BadRequest("Date cannot be greater than current date");
        }
        
        var rate = await _currencyRateService.GetRateAsync(date.Value, cancellationToken);
        return Ok(new { Rate = rate });
    }

    [HttpGet("average")]
    public async Task<IActionResult> GetAverage([FromQuery] DateTime? start, [FromQuery] DateTime? end, CancellationToken cancellationToken)
    {
        if (start == null || end == null)
        {
            return BadRequest("Start and end dates are required");
        }
        
        if (start.Value > end.Value)
        {
            return BadRequest("Start date cannot be greater than the end date");
        }
        
        var averageRate = await _currencyRateService.GetAverageRateAsync(start.Value, end.Value, cancellationToken);
        return Ok(new { Rate = averageRate });
    }
}
