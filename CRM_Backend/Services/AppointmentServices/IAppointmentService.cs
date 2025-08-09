using CRM_Backend.Models.DTOs.Appointment;
using CRM_Backend.Models.DTOs.Pagination;
using CRM_Backend.Models.Entities;

namespace CRM_Backend.Services.AppointmentServices;

public interface IAppointmentService
{
    Task<PaginatedListDto<GetAllAppointmentsPaginationResponse>> GetAllAppointmentsAsync(PaginationRequest request);
    Task<List<GetAppointmentsByDateResponse>> GetAppointmentsByDateAsync(DateTime date);
    Task<List<GetAllAppointmentsCalendarResponse>> GetAllAppointmentsCalendarAsync();
    Task<List<DateTime>> GetAllAppointmentDatesAsync();
    Task<GetAppointmentByIdResponse> GetAppointmentByIdAsync(int id);
    Task<bool> CreateAppointmentAsync(CreateAppointmentRequest request);
    Task<bool> UpdateAppointmentAsync(int id, UpdateAppointmentRequest request);
    Task<bool> DeleteAppointmentAsync(int id);
    Task<bool> MarkAppointmentAsFinishedAsync(int id);
    Task<bool> CancelAppointmentAsync(int id);
    Task<bool> SetAppointmentDurationAsync(TimeOnly time);
    Task<TimeOnly?> GetAppointmentDurationAsync();
}
