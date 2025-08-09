using CRM_Backend.Models.DTOs.Medicine;
using CRM_Backend.Models.Entities;
using CRM_Backend.Services.MedicineServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MedicineController(IMedicineService medicineService) : ControllerBase
{
    private readonly IMedicineService _medicineService = medicineService;

    [HttpGet("get-medicines")]
    public async Task<ActionResult<List<GetMedicinesResponse>>> GetMedicines()
    {
        var medicines = await _medicineService.GetMedicinesAsync();
        return Ok(medicines);
    }


    [HttpPost("create-medicine")]
    public async Task<ActionResult<Medicine>> CreateMedicine([FromBody] CreateMedicineRequest medicine)
    {
        var result = await _medicineService.CreateMedicineAsync(medicine);

        if (result == 0) return BadRequest();

        return Ok(new { id = result });
    }

    [HttpPut("update-medicine/{id}")]
    public async Task<IActionResult> UpdateMedicine(int id, [FromBody] UpdateMedicineRequest request)
    {

        var result = await _medicineService.UpdateMedicineAsync(id, request);
        if (!result) return NotFound();


        return Ok("Successfully updated");
    }

    [HttpDelete("delete-medicine/{id}")]
    public async Task<IActionResult> DeleteMedicine(int id)
    {
        var result = await _medicineService.DeleteMedicineAsync(id);

        if (!result) return NotFound();

        return Ok("Deleted");
    }
}
