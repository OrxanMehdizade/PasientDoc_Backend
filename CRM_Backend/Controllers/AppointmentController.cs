using CRM_Backend.Models.DTOs.Appointment;
using CRM_Backend.Models.DTOs.Pagination;
using CRM_Backend.Models.Entities;
using CRM_Backend.Services.AppointmentServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AppointmentController(IAppointmentService appointmentService) : ControllerBase
{
    private readonly IAppointmentService _appointmentService = appointmentService;

    [HttpGet("get-appointments")]
    public async Task<ActionResult<PaginatedListDto<GetAllAppointmentsPaginationResponse>>> GetAppointments([FromQuery] PaginationRequest paginationRequest)
    {
        var appointments = await _appointmentService.GetAllAppointmentsAsync(paginationRequest);

        return Ok(appointments);
    }

    [HttpGet("get-appointments-calendar")]
    public async Task<ActionResult<GetAllAppointmentsCalendarResponse>> GetAppointmentsCalendar()
    {
        var appointments = await _appointmentService.GetAllAppointmentsCalendarAsync();

        return Ok(appointments);
    }

    [HttpGet("get-appointment/{id}")]
    public async Task<ActionResult<GetAppointmentByIdResponse>> GetAppointment(int id)
    {
        var appointment = await _appointmentService.GetAppointmentByIdAsync(id);

        if (appointment == null) return NotFound();

        return Ok(appointment);
    }

    [HttpPost("create-appointment")]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest request)
    {
        var result = await _appointmentService.CreateAppointmentAsync(request);
        if (!result) return BadRequest("error occured while creating appointment");
        return Ok();
    }

    [HttpPut("update-appointment/{id}")]
    public async Task<IActionResult> UpdateAppointment(int id, [FromBody] UpdateAppointmentRequest request)
    {
        var result = await _appointmentService.UpdateAppointmentAsync(id, request);
        if (!result) return NotFound();

        return Ok(result);
    }

    [HttpDelete("delete-appointment/{id}")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        var result = await _appointmentService.DeleteAppointmentAsync(id);
        if (!result) return NotFound();

        return Ok("Deleted");
    }

    [HttpPut("mark-appointment-as-finished/{id}")]
    public async Task<IActionResult> MarkAppointmentAsFinished(int id)
    {
        var result = await _appointmentService.MarkAppointmentAsFinishedAsync(id);
        if (!result) return NotFound();

        return Ok();
    }

    [HttpPut("cancel-appointment/{id}")]
    public async Task<IActionResult> CancelAppointment(int id)
    {
        var result = await _appointmentService.CancelAppointmentAsync(id);
        if (!result) return NotFound();
        return Ok();
    }

    [HttpPut("set-appointment-duration")]
    public async Task<IActionResult> SetAppointmentDuration([FromBody] TimeOnly time)
    {
        var result = await _appointmentService.SetAppointmentDurationAsync(time);
        if (!result) return NotFound();

        return Ok();
    }

    [HttpGet("get-appointment-duration")]
    public async Task<ActionResult<TimeOnly>> GetAppointmentDuration()
    {
        var result = await _appointmentService.GetAppointmentDurationAsync();
        if (result == TimeOnly.MinValue) return NotFound();

        return Ok(result);
    }

    [HttpGet("get-appointments-by-date")]
    public async Task<ActionResult<List<GetAppointmentsByDateResponse>>> GetAppointmentsByDate([FromQuery] DateTime date)
    {
        var appointments = await _appointmentService.GetAppointmentsByDateAsync(date);
        return Ok(appointments);
    }

    [HttpGet("get-all-appointment-dates")]
    public async Task<ActionResult<List<DateTime>>> GetAllAppointmentDates()
    {
        var dates = await _appointmentService.GetAllAppointmentDatesAsync();
        return Ok(dates);
    }
}