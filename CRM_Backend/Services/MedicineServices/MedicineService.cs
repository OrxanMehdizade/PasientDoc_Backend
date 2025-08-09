using CRM_Backend.Data;
using CRM_Backend.Models.DTOs.Medicine;
using CRM_Backend.Models.Entities;
using CRM_Backend.Providers;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Services.MedicineServices;

public class MedicineService(CRMDbContext context, IDoctorProvider doctorProvider) : IMedicineService
{
    private readonly CRMDbContext _context = context;
    private readonly IDoctorProvider _doctorProvider = doctorProvider;

    public async Task<List<GetMedicinesResponse>> GetMedicinesAsync()
    {
        var doctorId = _doctorProvider.GetDoctorId();
        return await _context.Medicines
            .Where(m => m.DoctorId == doctorId)
            .Select(m => new GetMedicinesResponse { Id = m.Id, Name = m.Name })
            .ToListAsync();
    }

    public async Task<int> CreateMedicineAsync(CreateMedicineRequest request)
    {
        try
        {
            var doctorId = _doctorProvider.GetDoctorId();
            var newMedicine = new Medicine
            {
                Name = request.Name,
                DoctorId = doctorId
            };
            _context.Medicines.Add(newMedicine);
            await _context.SaveChangesAsync();
            return newMedicine.Id;

        }
        catch (Exception)
        {
            return 0;
        }
    }

    public async Task<bool> UpdateMedicineAsync(int id, UpdateMedicineRequest request)
    {
        var doctorId = _doctorProvider.GetDoctorId();

        var existingMedicine = await _context.Medicines.FirstOrDefaultAsync(m => m.Id == id && m.DoctorId == doctorId);

        if (existingMedicine == null) return false;

        existingMedicine.Name = request.Name;


        _context.Entry(existingMedicine).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await MedicineExists(existingMedicine.Id)) return false;

            else throw;

        }
    }

    public async Task<bool> DeleteMedicineAsync(int id)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var medicine = await _context.Medicines.FirstOrDefaultAsync(m => m.Id == id && m.DoctorId == doctorId);
        if (medicine == null) return false;


        _context.Medicines.Remove(medicine);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<bool> MedicineExists(int id)
    {
        return await _context.Medicines.AnyAsync(e => e.Id == id);
    }
}
