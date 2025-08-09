using CRM_Backend.Models.DTOs.Pagination;
using CRM_Backend.Models.DTOs.Treatment;
using CRM_Backend.Services.TreatmentServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TreatmentController(ITreatmentService treatmentService) : ControllerBase
    {
        private readonly ITreatmentService _treatmentService = treatmentService;

        [HttpGet("get-treatments")]
        public async Task<ActionResult<PaginatedListDto<GetTreatmentsResponse>>> GetTreatments([FromQuery] PaginationRequest paginationRequest)
        {
            var treatments = await _treatmentService.GetTreatmentsAsync(paginationRequest);
            return Ok(treatments);
        }

        [HttpGet("get-treatment/{id}")]
        public async Task<ActionResult<GetTreatmentByIdResponse>> GetTreatment(int id)
        {
            var treatment = await _treatmentService.GetTreatmentByIdAsync(id);

            if (treatment == null) return NotFound();

            return Ok(treatment);
        }

        [HttpPost("create-treatment")]
        public async Task<IActionResult> CreateTreatment([FromBody] CreateTreatmentRequest request)
        {
            var result = await _treatmentService.CreateTreatmentAsync(request);
            if (!result) return BadRequest("Failed to create treatment.");

            return Ok("Successfully Created");
        }

        [HttpPut("update-treatment/{id}")]
        public async Task<IActionResult> UpdateTreatment(int id, [FromBody] UpdateTreatmentRequest request)
        {
            var result = await _treatmentService.UpdateTreatmentAsync(id, request);
            if (!result) return NotFound();

            return Ok(result);
        }

        [HttpDelete("delete-treatment/{id}")]
        public async Task<IActionResult> DeleteTreatment(int id)
        {
            var result = await _treatmentService.DeleteTreatmentAsync(id);
            if (!result) return NotFound();

            return Ok("Deleted");
        }
    }
}
