using CRM_Backend.Data;
using CRM_Backend.Models.DTOs.Appointment;
using CRM_Backend.Models.DTOs.Pagination;
using CRM_Backend.Models.Entities;
using CRM_Backend.Providers;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Services.AppointmentServices;

public class AppointmentService(CRMDbContext context, IDoctorProvider doctorProvider) : IAppointmentService
{
    private readonly CRMDbContext _context = context;
    private readonly IDoctorProvider _doctorProvider = doctorProvider;

    public async Task<PaginatedListDto<GetAllAppointmentsPaginationResponse>> GetAllAppointmentsAsync(PaginationRequest request)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var query = _context.Appointments
            .Where(a => a.DoctorId == doctorId)
            .Include(a => a.Patient)
            .Include(a => a.Service)
            .AsQueryable();

        var totalItems = await query.CountAsync();

        var appointments = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var appointmentDtos = appointments.Select(a => new GetAllAppointmentsPaginationResponse
        {
            Id = a.Id,
            PatientName = $"{a.Patient.Name} {a.Patient.Surname}",
            Phone = a.Patient.PhoneNumber,
            Service = a.Service.Name,
            Date = a.Date.ToString("yyyy-MM-dd"),
            Time = a.Date.ToString("HH:mm"),
            Hospital = a.Hospital,
            Status = a.IsCancelled ? "Cancelled" : a.IsFinished ? "Finished" : "Pending"
        }).ToList();

        var paginationMeta = new PaginationMeta(request.Page, request.PageSize, totalItems);

        return new PaginatedListDto<GetAllAppointmentsPaginationResponse>(appointmentDtos, paginationMeta);
    }

    public async Task<List<GetAllAppointmentsCalendarResponse>> GetAllAppointmentsCalendarAsync()
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var appointments = await _context.Appointments
            .Where(a => a.DoctorId == doctorId)
            .Include(a => a.Patient)
            .Include(a => a.Service)
            .ToListAsync();

        var appointmentDtos = appointments.Select(a => new GetAllAppointmentsCalendarResponse
        {
            Title = $"{a.Patient.Name} {a.Patient.Surname} - {a.Patient.PhoneNumber}",
            Date = a.Date
        }).ToList();

        return appointmentDtos;
    }

    public async Task<List<GetAppointmentsByDateResponse>> GetAppointmentsByDateAsync(DateTime date)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var appointments = await _context.Appointments
            .Where(a => a.DoctorId == doctorId && a.Date.Date == date.Date)
            .Include(a => a.Patient)
            .Include(a => a.Service)
            .ToListAsync();

        var appointmentDtos = appointments.Select(a => new GetAppointmentsByDateResponse
        {
            Id = a.Id,
            PatientFullName = $"{a.Patient.Name} {a.Patient.Surname}",
            PhoneNumber = a.Patient.PhoneNumber,
            ServiceName = a.Service.Name,
            Time = a.Date.ToString("HH:mm"),
            Status = a.IsCancelled ? "Cancelled" : a.IsFinished ? "Finished" : "Pending"
        }).ToList();

        return appointmentDtos;
    }

    public async Task<List<DateTime>> GetAllAppointmentDatesAsync()
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var dates = await _context.Appointments
            .Where(a => a.DoctorId == doctorId)
            .Select(a => a.Date.Date)
            .Distinct()
            .ToListAsync();

        return dates;
    }

    public async Task<GetAppointmentByIdResponse> GetAppointmentByIdAsync(int id)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var appointment = await _context.Appointments
            .Where(a => a.Id == id && a.DoctorId == doctorId)
            .Include(a => a.Patient)
            .Include(a => a.Service)
            .FirstOrDefaultAsync();

        if (appointment == null) return null!;

        return new GetAppointmentByIdResponse
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            ServiceId = appointment.ServiceId,
            HospitalName = appointment.Hospital,
            Date = DateOnly.FromDateTime(appointment.Date),
            Time = TimeOnly.FromDateTime(appointment.Date),
            Appeal = appointment.Appeal,
            Status = appointment.IsCancelled ? "Cancelled" : appointment.IsFinished ? "Finished" : "Pending"
        };
    }

    public async Task<bool> CreateAppointmentAsync(CreateAppointmentRequest request)
    {
        try
        {
            var doctorId = _doctorProvider.GetDoctorId();
            var appointmentDateTime = new DateTime(request.Date.Year, request.Date.Month, request.Date.Day, request.Time.Hour, request.Time.Minute, request.Time.Second);

            var appointment = new Appointment
            {
                PatientId = request.PatientId,
                ServiceId = request.ServiceId,
                Hospital = request.HospitalName,
                Date = appointmentDateTime,
                Appeal = request.Appeal,
                DoctorId = doctorId,
                IsPending = true,
                IsFinished = false,
                IsCancelled = false
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdateAppointmentAsync(int id, UpdateAppointmentRequest request)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id && a.DoctorId == doctorId);

        if (appointment == null) return false;

        appointment.PatientId = request.PatientId;
        appointment.ServiceId = request.ServiceId;
        appointment.Hospital = request.HospitalName;
        appointment.Date = new DateTime(request.Date.Year, request.Date.Month, request.Date.Day, request.Time.Hour, request.Time.Minute, request.Time.Second);
        appointment.Appeal = request.Appeal;
        appointment.IsCancelled = request.Status == "Cancelled";
        appointment.IsFinished = request.Status == "Finished";
        appointment.IsPending = request.Status == "Pending";

        _context.Entry(appointment).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await AppointmentExists(appointment.Id)) return false;
            else throw;
        }
    }

    public async Task<bool> DeleteAppointmentAsync(int id)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var appointment = await _context.Appointments
            .Where(a => a.Id == id && a.DoctorId == doctorId)
            .FirstOrDefaultAsync();

        if (appointment == null) return false;

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAppointmentAsFinishedAsync(int id)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var appointment = await _context.Appointments
            .Where(a => a.Id == id && a.DoctorId == doctorId)
            .FirstOrDefaultAsync();

        if (appointment == null) return false;

        appointment.IsFinished = true;
        appointment.IsPending = false;
        _context.Entry(appointment).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelAppointmentAsync(int id)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var appointment = await _context.Appointments
            .Where(a => a.Id == id && a.DoctorId == doctorId)
            .FirstOrDefaultAsync();

        if (appointment == null) return false;

        appointment.IsCancelled = true;
        appointment.IsPending = false;
        _context.Entry(appointment).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetAppointmentDurationAsync(TimeOnly time)
    {
        var doctorId = _doctorProvider.GetDoctorId();

        var doctor = await _context.Users.FirstOrDefaultAsync(d => d.Id == doctorId);

        if (doctor is null) return false;

        doctor!.AppointmentDuration = time;

        _context.Entry(doctor).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TimeOnly?> GetAppointmentDurationAsync()
    {
        var doctorId = _doctorProvider.GetDoctorId();

        var doctor = await _context.Users.FirstOrDefaultAsync(d => d.Id == doctorId);

        if (doctor is null) return TimeOnly.MinValue;

        return doctor!.AppointmentDuration;
    }

    private async Task<bool> AppointmentExists(int id) => await _context.Appointments.AnyAsync(e => e.Id == id);
}
