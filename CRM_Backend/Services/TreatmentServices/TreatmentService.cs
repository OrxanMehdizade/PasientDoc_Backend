using CRM_Backend.Data;
using CRM_Backend.Models.DTOs.Diagnose;
using CRM_Backend.Models.DTOs.Medicine;
using CRM_Backend.Models.DTOs.Pagination;
using CRM_Backend.Models.DTOs.Treatment;
using CRM_Backend.Models.DTOs.TreatmentForm;
using CRM_Backend.Models.Entities;
using CRM_Backend.Providers;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Services.TreatmentServices;

public class TreatmentService(CRMDbContext context, IDoctorProvider doctorProvider) : ITreatmentService
{
    private readonly CRMDbContext _context = context;
    private readonly IDoctorProvider _doctorProvider = doctorProvider;

    public async Task<PaginatedListDto<GetTreatmentsResponse>> GetTreatmentsAsync(PaginationRequest paginationRequest)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var query = _context.Treatments
            .Where(t => t.DoctorId == doctorId)
            .Include(t => t.Service)
            .Include(t => t.DiagnoseTreatments).ThenInclude(dt => dt.Diagnose)
            .Include(t => t.MedicineTreatments).ThenInclude(mt => mt.Medicine)
            .Include(t => t.Patient)
            .AsQueryable();

        var totalItems = await query.CountAsync();
        var treatments = await query
            .Skip((paginationRequest.Page - 1) * paginationRequest.PageSize)
            .Take(paginationRequest.PageSize)
            .ToListAsync();

        var treatmentResponses = treatments.Select(t => new GetTreatmentsResponse
        {
            Id = t.Id,
            PatientName = $"{t.Patient.Name} {t.Patient.Surname}",
            Date = t.Date,
            ServiceName = t.Service?.Name,
            DiagnoseNames = t.DiagnoseTreatments.Select(dt => dt.Diagnose.Name).ToList()
        }).ToList();

        var paginationMeta = new PaginationMeta(paginationRequest.Page, paginationRequest.PageSize, totalItems);

        return new PaginatedListDto<GetTreatmentsResponse>(treatmentResponses, paginationMeta);
    }


    public async Task<GetTreatmentByIdResponse> GetTreatmentByIdAsync(int id)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var treatment = await _context.Treatments
            .Where(t => t.Id == id && t.DoctorId == doctorId)
            .Include(t => t.Service)
            .Include(t => t.DiagnoseTreatments).ThenInclude(dt => dt.Diagnose)
            .Include(t => t.MedicineTreatments).ThenInclude(mt => mt.Medicine)
            .Include(t => t.TreatmentFormContents)!.ThenInclude(tfc => tfc.TreatmentForm)
            .Include(t => t.Patient)
            .FirstOrDefaultAsync();

        if (treatment == null) return null!;

        var response = new GetTreatmentByIdResponse
        {
            Id = treatment.Id,
            PatientId = treatment.PatientId,
            PatientName = $"{treatment.Patient.Name} {treatment.Patient.Surname} - {treatment.Patient.PhoneNumber}",
            ServiceId = treatment.ServiceId,
            ServiceName = treatment.Service?.Name,
            Diagnoses = treatment.DiagnoseTreatments.Select(dt => new GetDiagnosesResponse
            {
                Id = dt.DiagnoseId,
                Name = dt.Diagnose.Name
            }).ToList(),
            Medicines = treatment.MedicineTreatments.Select(mt => new GetMedicinesResponse
            {
                Id = mt.MedicineId,
                Name = mt.Medicine.Name
            }).ToList(),
            TreatmentForms = treatment.TreatmentFormContents!.Select(tfc => new TreatmentFormDto
            {
                Id = tfc.TreatmentFormId,
                Name = tfc.TreatmentForm.Name,
                Content = tfc.Content!
            }).ToList(),
            DocumentUrl = treatment.DocumentUrl
        };

        return response;
    }

    public async Task<bool> CreateTreatmentAsync(CreateTreatmentRequest request)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == request.PatientId && p.DoctorId == doctorId);

        if (patient == null) return false;

        var service = await _context.Services.FindAsync(request.ServiceId);
        if (service == null) return false;

        var treatment = new Treatment
        {
            PatientId = request.PatientId,
            ServiceId = request.ServiceId,
            DoctorId = doctorId,
            Date = DateTime.Now,
            DocumentUrl = request.DocumentUrl
        };

        var diagnoses = await _context.Diagnoses.Where(d => request.DiagnoseIds.Contains(d.Id)).ToListAsync();
        foreach (var diagnose in diagnoses)
        {
            var diagnoseTreatment = new DiagnoseTreatment
            {
                DiagnoseId = diagnose.Id,
                Treatment = treatment
            };
            _context.DiagnoseTreatments.Add(diagnoseTreatment);
            treatment.DiagnoseTreatments.Add(diagnoseTreatment);
        }

        var medicines = await _context.Medicines.Where(m => request.MedicineIds.Contains(m.Id)).ToListAsync();
        foreach (var medicine in medicines)
        {
            var medicineTreatment = new MedicineTreatment
            {
                MedicineId = medicine.Id,
                Treatment = treatment
            };
            _context.MedicineTreatments.Add(medicineTreatment);
            treatment.MedicineTreatments.Add(medicineTreatment);
        }

        foreach (var formDto in request.TreatmentForms)
        {
            var treatmentFormContent = new TreatmentFormContent
            {
                Content = formDto.Content,
                DoctorId = doctorId,
                TreatmentFormId = formDto.TreatmentFormId,
                Treatment = treatment
            };

            _context.TreatmentFormContents.Add(treatmentFormContent);
            treatment.TreatmentFormContents.Add(treatmentFormContent);
        }

        _context.Treatments.Add(treatment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateTreatmentAsync(int id, UpdateTreatmentRequest request)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var treatment = await _context.Treatments
            .Where(t => t.Id == id && t.DoctorId == doctorId)
            .Include(t => t.DiagnoseTreatments)
            .Include(t => t.MedicineTreatments)
            .Include(t => t.TreatmentFormContents)
            .FirstOrDefaultAsync();

        if (treatment == null) return false;

        treatment.PatientId = request.PatientId;
        treatment.ServiceId = request.ServiceId;
        treatment.Date = DateTime.Now;
        treatment.DocumentUrl = request.DocumentUrl;

        // Update DiagnoseTreatments
        var existingDiagnoseIds = treatment.DiagnoseTreatments.Select(dt => dt.DiagnoseId).ToList();
        var newDiagnoseIds = request.DiagnoseIds.Except(existingDiagnoseIds).ToList();
        var removedDiagnoseIds = existingDiagnoseIds.Except(request.DiagnoseIds).ToList();

        foreach (var diagnoseId in removedDiagnoseIds)
        {
            var diagnoseTreatment = treatment.DiagnoseTreatments.FirstOrDefault(dt => dt.DiagnoseId == diagnoseId);
            if (diagnoseTreatment != null)
            {
                _context.DiagnoseTreatments.Remove(diagnoseTreatment);
            }
        }

        foreach (var diagnoseId in newDiagnoseIds)
        {
            var diagnoseTreatment = new DiagnoseTreatment
            {
                DiagnoseId = diagnoseId,
                TreatmentId = treatment.Id
            };
            _context.DiagnoseTreatments.Add(diagnoseTreatment);
        }

        // Update MedicineTreatments
        var existingMedicineIds = treatment.MedicineTreatments.Select(mt => mt.MedicineId).ToList();
        var newMedicineIds = request.MedicineIds.Except(existingMedicineIds).ToList();
        var removedMedicineIds = existingMedicineIds.Except(request.MedicineIds).ToList();

        foreach (var medicineId in removedMedicineIds)
        {
            var medicineTreatment = treatment.MedicineTreatments.FirstOrDefault(mt => mt.MedicineId == medicineId);
            if (medicineTreatment != null)
            {
                _context.MedicineTreatments.Remove(medicineTreatment);
            }
        }

        foreach (var medicineId in newMedicineIds)
        {
            var medicineTreatment = new MedicineTreatment
            {
                MedicineId = medicineId,
                TreatmentId = treatment.Id
            };
            _context.MedicineTreatments.Add(medicineTreatment);
        }

        // Update TreatmentFormContents
        var existingFormContents = treatment.TreatmentFormContents.ToList();
        var requestFormContents = request.TreatmentForms.ToList();

        foreach (var existingFormContent in existingFormContents)
        {
            var matchingFormContent = requestFormContents.FirstOrDefault(rfc => rfc.TreatmentFormId == existingFormContent.TreatmentFormId);

            if (matchingFormContent == null)
            {
                _context.TreatmentFormContents.Remove(existingFormContent);
            }
            else
            {
                existingFormContent.Content = matchingFormContent.Content;
                _context.TreatmentFormContents.Update(existingFormContent);
                requestFormContents.Remove(matchingFormContent);
            }
        }

        foreach (var newFormContent in requestFormContents)
        {
            var treatmentFormContent = new TreatmentFormContent
            {
                Content = newFormContent.Content,
                DoctorId = doctorId,
                TreatmentFormId = newFormContent.TreatmentFormId,
                TreatmentId = treatment.Id
            };
            _context.TreatmentFormContents.Add(treatmentFormContent);
        }

        await _context.SaveChangesAsync();
        return true;
    }




    public async Task<bool> DeleteTreatmentAsync(int id)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var treatment = await _context.Treatments
            .Where(t => t.Id == id && t.DoctorId == doctorId)
            .Include(t => t.TreatmentFormContents)
            .FirstOrDefaultAsync();

        if (treatment == null) return false;

        _context.TreatmentFormContents.RemoveRange(treatment.TreatmentFormContents);

        _context.Treatments.Remove(treatment);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<bool> TreatmentExists(int id) => await _context.Treatments.AnyAsync(e => e.Id == id);
}
