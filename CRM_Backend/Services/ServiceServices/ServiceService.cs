using CRM_Backend.Data;
using CRM_Backend.Models.DTOs.Service;
using CRM_Backend.Models.DTOs.TreatmentForm;
using CRM_Backend.Models.Entities;
using CRM_Backend.Providers;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Services.ServiceServices;

public class ServiceService(CRMDbContext context, IDoctorProvider doctorProvider) : IServiceService
{
    private readonly CRMDbContext _context = context;
    private readonly IDoctorProvider _doctorProvider = doctorProvider;

    public async Task<List<GetServicesResponse>> GetServicesAsync()
    {
        var doctorId = _doctorProvider.GetDoctorId();
        return await _context.Services
            .Where(s => s.DoctorId == doctorId)
            .Select(s => new GetServicesResponse { Id = s.Id, Name = s.Name })
            .ToListAsync();
    }

    public async Task<int> CreateServiceAsync(CreateServiceRequest request)
    {
        try
        {
            var doctorId = _doctorProvider.GetDoctorId();
            var newService = new Service
            {
                Name = request.Name,
                DoctorId = doctorId
            };
            _context.Services.Add(newService);
            await _context.SaveChangesAsync();
            return newService.Id;

        }
        catch (Exception)
        {
            return 0;
        }
    }

    public async Task<bool> UpdateServiceAsync(int id, UpdateServiceRequest request)
    {
        var doctorId = _doctorProvider.GetDoctorId();

        var existingService = await _context.Services
           .FirstOrDefaultAsync(s => s.Id == id && s.DoctorId == doctorId);

        if (existingService == null) return false;

        existingService.Name = request.Name;

        _context.Entry(existingService).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ServiceExists(existingService.Id)) return false;

            else throw;

        }
    }

    public async Task<bool> DeleteServiceAsync(int id)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == id && s.DoctorId == doctorId);
        if (service == null) return false;


        _context.Services.Remove(service);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<bool> ServiceExists(int id) => await _context.Services.AnyAsync(e => e.Id == id);
}
