using CRM_Backend.Data;
using CRM_Backend.Models.DTOs.Statistics;
using CRM_Backend.Models.Enums;
using CRM_Backend.Providers;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Services.StatisticsService;

public class StatisticsService(CRMDbContext context, IDoctorProvider doctorProvider) : IStatisticsService
{
    private readonly CRMDbContext _context = context;
    private readonly IDoctorProvider _doctorProvider = doctorProvider;

    public async Task<GeneralStatisticsDto> GetGeneralStatisticsAsync(StatisticsFilterDto filter)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var appointmentsQuery = _context.Appointments.Where(a => a.DoctorId == doctorId).AsQueryable();
        var treatmentsQuery = _context.Treatments.Where(t => t.DoctorId == doctorId).AsQueryable();
        var patientsQuery = _context.Patients.Where(p => p.DoctorId == doctorId).AsQueryable();

        if (filter.StartDate.HasValue)
        {
            appointmentsQuery = appointmentsQuery.Where(a => a.Date >= filter.StartDate);
            treatmentsQuery = treatmentsQuery.Where(t => t.Date >= filter.StartDate);
            patientsQuery = patientsQuery.Where(p => p.CreatedTime >= filter.StartDate);
        }

        if (filter.EndDate.HasValue)
        {
            appointmentsQuery = appointmentsQuery.Where(a => a.Date <= filter.EndDate);
            treatmentsQuery = treatmentsQuery.Where(t => t.Date <= filter.EndDate);
            patientsQuery = patientsQuery.Where(p => p.CreatedTime <= filter.EndDate);
        }

        var totalAppointments = await appointmentsQuery.CountAsync();
        var totalTreatments = await treatmentsQuery.CountAsync();
        var totalPatients = await patientsQuery.CountAsync();

        return new GeneralStatisticsDto
        {
            TotalAppointments = totalAppointments,
            TotalTreatments = totalTreatments,
            TotalPatients = totalPatients
        };
    }

    public async Task<AppealStatisticsDto> GetPatientsComeFromAsync(StatisticsFilterDto filter)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var patientsQuery = _context.Patients.Where(p => p.DoctorId == doctorId).AsQueryable();

        if (filter.StartDate.HasValue)
        {
            patientsQuery = patientsQuery.Where(p => p.CreatedTime >= filter.StartDate);
        }

        if (filter.EndDate.HasValue)
        {
            patientsQuery = patientsQuery.Where(p => p.CreatedTime <= filter.EndDate);
        }

        var appealCounts = await patientsQuery
            .Join(_context.Appointments, p => p.Id, a => a.PatientId, (p, a) => new { p, a.Appeal })
            .GroupBy(pa => pa.Appeal)
            .Select(g => new { Appeal = g.Key, Count = g.Count() })
            .ToListAsync();

        return new AppealStatisticsDto
        {
            SocialMedia = appealCounts.FirstOrDefault(a => a.Appeal == Appeal.SocialMedia)?.Count ?? 0,
            DoctorReferral = appealCounts.FirstOrDefault(a => a.Appeal == Appeal.DoctorReferral)?.Count ?? 0,
            ClinicReferral = appealCounts.FirstOrDefault(a => a.Appeal == Appeal.ClinicReferral)?.Count ?? 0,
            Personal = appealCounts.FirstOrDefault(a => a.Appeal == Appeal.Personal)?.Count ?? 0
        };
    }
}
