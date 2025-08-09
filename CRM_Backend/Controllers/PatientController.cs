using CRM_Backend.Models.DTOs.Pagination;
using CRM_Backend.Models.DTOs.Patient;
using CRM_Backend.Models.Entities;
using CRM_Backend.Services.PatientServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PatientController(IPatientService patientService) : ControllerBase
{
    private readonly IPatientService _patientService = patientService;

    [HttpGet("get-all-patients")]
    public async Task<ActionResult<PaginatedListDto<GetAllPatientsResponse>>> GetAllPatients([FromQuery] PaginationRequest paginationRequest)
    {
        var paginatedPatients = await _patientService.GetPatientsAsync(paginationRequest);
        return Ok(paginatedPatients);
    }

    [HttpGet("get-all-search-patients")]
    public async Task<ActionResult<List<GetAllSearchPatientsResponse>>> GetAllSearchPatients()
    {
        var patients = await _patientService.GetSearchPatientsAsync();
        return Ok(patients);
    }

    [HttpGet("get-patient/{id}")]
    public async Task<ActionResult<GetPatientByIdResponse>> GetPatient(int id)
    {
        var patient = await _patientService.GetPatientByIdAsync(id);

        if (patient == null) return NotFound();


        return Ok(patient);
    }

    [HttpPut("update-patient/{id}")]
    public async Task<IActionResult> UpdatePatient(int id, [FromBody] UpdatePatientRequest request)
    {
        var result = await _patientService.UpdatePatientAsync(id, request);
        if (!result) return NotFound();

        return Ok(result);
    }


    [HttpPost("create-patient")]
    public async Task<ActionResult<Patient>> CreatePatient(CreatePatientDto patientDto)
    {


        var createdPatient = await _patientService.CreatePatientAsync(patientDto);
        return Ok("Successfully Created");
    }

    [HttpDelete("delete-patient/{id}")]
    public async Task<IActionResult> DeletePatient(int id)
    {
        var result = await _patientService.DeletePatientAsync(id);
        if (!result) return NotFound();


        return Ok("Deleted");
    }
}
