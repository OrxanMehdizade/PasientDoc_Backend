using CRM_Backend.Models.DTOs.TreatmentForm;
using CRM_Backend.Models.Entities;
using CRM_Backend.Services.TreatmentFormServices;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TreatmentFormController(ITreatmentFormService treatmentFormService) : ControllerBase
{
    private readonly ITreatmentFormService _treatmentFormService = treatmentFormService;

    [HttpGet("get-treatmentforms")]
    public async Task<ActionResult<List<GetTreatFormsDto>>> GetTreatmentForms()
    {
        var treatmentForms = await _treatmentFormService.GetTreatmentFormsAsync();
        return Ok(treatmentForms);
    }

    [HttpGet("get-treatmentform/{id}")]
    public async Task<ActionResult<TreatmentForm>> GetTreatmentForm(int id)
    {
        var treatmentForm = await _treatmentFormService.GetTreatmentFormByIdAsync(id);

        if (treatmentForm == null) return NotFound();

        return Ok(treatmentForm);
    }

    [HttpPost("create-treatmentform")]
    public async Task<IActionResult> CreateTreatmentForm([FromBody] CreateTreatmentFormRequest treatmentForm)
    {
        var result = await _treatmentFormService.CreateTreatmentFormAsync(treatmentForm);
        if (result == 0) return BadRequest();


        return Ok(new { id = result });
    }

    [HttpPut("update-treatmentform/{id}")]
    public async Task<IActionResult> UpdateTreatmentForm(int id, [FromBody] UpdateTreatmentFormRequest treatmentForm)
    {
        var result = await _treatmentFormService.UpdateTreatmentFormAsync(id, treatmentForm);
        if (!result) return NotFound();

        return Ok("Successfully updated");
    }

    [HttpDelete("delete-treatmentform/{id}")]
    public async Task<IActionResult> DeleteTreatmentForm(int id)
    {
        var result = await _treatmentFormService.DeleteTreatmentFormAsync(id);
        if (!result) return NotFound();

        return Ok("Deleted");
    }
}
