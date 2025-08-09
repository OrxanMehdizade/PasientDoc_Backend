using CRM_Backend.Models.DTOs.Diagnose;
using CRM_Backend.Models.Entities;
using CRM_Backend.Services.DiagnoseServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DiagnoseController(IDiagnoseService diagnoseService) : ControllerBase
{
    private readonly IDiagnoseService _diagnoseService = diagnoseService;

    [HttpGet("get-diagnoses")]
    public async Task<ActionResult<List<GetDiagnosesResponse>>> GetDiagnoses()
    {
        var diagnoses = await _diagnoseService.GetDiagnosesAsync();
        return Ok(diagnoses);
    }

    [HttpPost("create-diagnose")]
    public async Task<IActionResult> CreateDiagnose([FromBody] CreateDiagnoseRequest request)
    {
        var result = await _diagnoseService.CreateDiagnoseAsync(request);
        if (result == 0) return BadRequest();
        return Ok(new { id = result });
    }

    [HttpPut("update-diagnose/{id}")]
    public async Task<IActionResult> UpdateDiagnose(int id, [FromBody] UpdateDiagnoseRequest request)
    {


        var result = await _diagnoseService.UpdateDiagnoseAsync(id, request);
        if (!result) return NotFound();

        return Ok();
    }

    [HttpDelete("delete-diagnose/{id}")]
    public async Task<IActionResult> DeleteDiagnose(int id)
    {
        var result = await _diagnoseService.DeleteDiagnoseAsync(id);
        if (!result) return NotFound();


        return Ok("Deleted");
    }
}
