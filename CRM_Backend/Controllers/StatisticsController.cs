using CRM_Backend.Models.DTOs.Statistics;
using CRM_Backend.Services.StatisticsService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class StatisticsController(IStatisticsService statisticsService) : ControllerBase
{
    private readonly IStatisticsService _statisticsService = statisticsService;

    [HttpGet("get-general-statistics")]
    public async Task<ActionResult<GeneralStatisticsDto>> GetGeneralStatistics([FromQuery] StatisticsFilterDto filter)
    {
        var statistics = await _statisticsService.GetGeneralStatisticsAsync(filter);
        return Ok(statistics);
    }

    [HttpGet("get-patients-come-from")]
    public async Task<ActionResult<AppealStatisticsDto>> GetPatientsComeFrom([FromQuery] StatisticsFilterDto filter)
    {
        var statistics = await _statisticsService.GetPatientsComeFromAsync(filter);
        return Ok(statistics);
    }
}
