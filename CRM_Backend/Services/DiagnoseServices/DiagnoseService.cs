using CRM_Backend.Data;
using CRM_Backend.Models.DTOs.Diagnose;
using CRM_Backend.Models.Entities;
using CRM_Backend.Providers;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Services.DiagnoseServices;

public class DiagnoseService(CRMDbContext context, IDoctorProvider doctorProvider) : IDiagnoseService
{
    private readonly CRMDbContext _context = context;
    private readonly IDoctorProvider _doctorProvider = doctorProvider;

    public async Task<List<GetDiagnosesResponse>> GetDiagnosesAsync()
    {
        var doctorId = _doctorProvider.GetDoctorId();
        return await _context.Diagnoses
            .Where(d => d.DoctorId == doctorId)
            .Select(d => new GetDiagnosesResponse { Id = d.Id, Name = d.Name })
            .ToListAsync();
    }

    public async Task<int> CreateDiagnoseAsync(CreateDiagnoseRequest request)
    {
        try
        {
            var doctorId = _doctorProvider.GetDoctorId();
            var newDiagnose = new Diagnose
            {
                Name = request.Name,
                DoctorId = doctorId
            };
            _context.Diagnoses.Add(newDiagnose);
            await _context.SaveChangesAsync();
            return newDiagnose.Id;
        }
        catch (Exception)
        {
            return 0;
        }

    }

    public async Task<bool> UpdateDiagnoseAsync(int id, UpdateDiagnoseRequest request)
    {
        var doctorId = _doctorProvider.GetDoctorId();

        var existingDiagnose = await _context.Diagnoses
         .FirstOrDefaultAsync(s => s.Id == id && s.DoctorId == doctorId);

        if (existingDiagnose == null) return false;

        existingDiagnose.Name = request.Name;

        _context.Entry(existingDiagnose).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await DiagnoseExists(existingDiagnose.Id)) return false;

            else throw;

        }
    }

    public async Task<bool> DeleteDiagnoseAsync(int id)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var diagnose = await _context.Diagnoses.FirstOrDefaultAsync(d => d.Id == id && d.DoctorId == doctorId);
        if (diagnose == null) return false;


        _context.Diagnoses.Remove(diagnose);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<bool> DiagnoseExists(int id)
    {
        return await _context.Diagnoses.AnyAsync(e => e.Id == id);
    }
}