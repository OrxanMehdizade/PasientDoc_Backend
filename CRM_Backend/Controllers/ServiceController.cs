using CRM_Backend.Models.DTOs.Service;
using CRM_Backend.Services.ServiceServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ServiceController(IServiceService serviceService) : ControllerBase
{
    private readonly IServiceService _serviceService = serviceService;

    [HttpGet("get-services")]
    public async Task<ActionResult<List<GetServicesResponse>>> GetServices()
    {
        var services = await _serviceService.GetServicesAsync();
        return Ok(services);
    }

    [HttpPost("create-service")]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest request)
    {
        var result = await _serviceService.CreateServiceAsync(request);
        if (result == 0) return BadRequest();
        return Ok(new { id = result });
    }

    [HttpPut("update-service/{id}")]
    public async Task<IActionResult> UpdateService(int id, [FromBody] UpdateServiceRequest request)
    {

        var result = await _serviceService.UpdateServiceAsync(id, request);
        if (!result) return NotFound();

        return Ok(result);
    }

    [HttpDelete("delete-service/{id}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var result = await _serviceService.DeleteServiceAsync(id);

        if (!result) return NotFound();

        return Ok("Deleted");
    }
}
