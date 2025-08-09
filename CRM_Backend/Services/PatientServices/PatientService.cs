using CRM_Backend.Data;
using CRM_Backend.Models.DTOs.Appointment;
using CRM_Backend.Models.DTOs.Pagination;
using CRM_Backend.Models.DTOs.Patient;
using CRM_Backend.Models.DTOs.Treatment;
using CRM_Backend.Models.Entities;
using CRM_Backend.Providers;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Services.PatientServices;

public class PatientService(CRMDbContext context, IDoctorProvider doctorProvider) : IPatientService
{
    private readonly CRMDbContext _context = context;
    private readonly IDoctorProvider _doctorProvider = doctorProvider;
    public async Task<PaginatedListDto<GetAllPatientsResponse>> GetPatientsAsync(PaginationRequest paginationRequest)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var query = _context.Patients.AsQueryable();

        // Apply pagination
        var totalItems = await query.CountAsync();
        var patients = await query
            .Skip((paginationRequest.Page - 1) * paginationRequest.PageSize)
            .Take(paginationRequest.PageSize)
            .ToListAsync();

        // Map to GetAllPatientsResponse
        var patientDtos = patients
            .Where(p => p.DoctorId == doctorId)
            .Select(patient => new GetAllPatientsResponse
            {
                Id = patient.Id,
                FullName = $"{patient.Name} {patient.Surname}",
                Phone = patient.PhoneNumber,
                Gender = patient.Gender.ToString(),
                Age = patient.BirthDate.HasValue ? DateTime.Now.Year - patient.BirthDate.Value.Year : 0
            }).ToList();

        var paginationMeta = new PaginationMeta(paginationRequest.Page, paginationRequest.PageSize, totalItems);

        return new PaginatedListDto<GetAllPatientsResponse>(patientDtos, paginationMeta);
    }

    public async Task<List<GetAllSearchPatientsResponse>> GetSearchPatientsAsync()
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var patients = _context.Patients.ToList();

        // Map to GetAllPatientsResponse
        var patientDtos = patients
            .Where(p => p.DoctorId == doctorId)
            .Select(patient => new GetAllSearchPatientsResponse
            {
                Id = patient.Id,
                FullNameWithPhone = $"{patient.Name} {patient.Surname} - {patient.PhoneNumber}"
            }).ToList();


        return patientDtos;
    }

    public async Task<GetPatientByIdResponse> GetPatientByIdAsync(int patientId)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var patient = await _context.Patients
            .Where(p => p.DoctorId == doctorId)
            .Include(p => p.Doctor)
            .Include(p => p.Appointments)
            .Include(p => p.Treatments)!
                .ThenInclude(t => t.Service)
            .FirstOrDefaultAsync(p => p.Id == patientId && p.DoctorId == doctorId);

        if (patient == null) return null!;

        var response = new GetPatientByIdResponse
        {
            Name = patient.Name,
            Surname = patient.Surname,
            FatherName = patient.FatherName,
            PhoneNumber = patient.PhoneNumber,
            BirthDate = patient.BirthDate,
            Gender = patient.Gender.ToString(),
            Address = patient.Address!,
            Email = patient.Email!,
            Insurance = patient.Insurance!,
            DoctorReferral = patient.DoctorReferral!,
            Appointments = patient.Appointments?.Select(a => new AppointmentDto
            {
                Id = a.Id,
                Date = a.Date,
                Status = a.IsCancelled ? "Cancelled" : a.IsFinished ? "Finished" : "Pending"
            }).ToList()!,
            Treatments = patient.Treatments?.Select(t => new TreatmentDto
            {
                Id = t.Id,
                Date = t.Date,
                ServiceName = t.Service.Name
            }).ToList()!
        };

        return response;
    }

    public async Task<Patient> CreatePatientAsync(CreatePatientDto patientDto)
    {
        var doctorId = _doctorProvider.GetDoctorId();

        var patient = new Patient
        {
            Name = patientDto.Name,
            Surname = patientDto.Surname,
            FatherName = patientDto.FatherName,
            PhoneNumber = patientDto.Phone,
            BirthDate = patientDto.Birthday,
            Gender = patientDto.Gender,
            Address = patientDto.Address,
            Email = patientDto.Email,
            Insurance = patientDto.Insurance,
            DoctorReferral = patientDto.DoctorReferral,
            DoctorId = doctorId
        };
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        return patient;
    }

    public async Task<bool> UpdatePatientAsync(int id, UpdatePatientRequest request)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == id && p.DoctorId == doctorId);

        if (patient == null) return false;

        // Update patient fields
        patient.Name = request.Name;
        patient.Surname = request.Surname;
        patient.FatherName = request.FatherName;
        patient.PhoneNumber = request.Phone;
        patient.BirthDate = request.BirthDay;
        patient.Gender = request.Gender;
        patient.Address = request.Address;
        patient.Email = request.Email;
        patient.Insurance = request.Insurance;
        patient.DoctorReferral = request.DoctorReferral;

        _context.Entry(patient).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await PatientExists(patient.Id)) return false;
            else throw;
        }
    }


    public async Task<bool> DeletePatientAsync(int patientId)
    {
        var doctorId = _doctorProvider.GetDoctorId();
        var patient = await _context.Patients
            .Include(p => p.Treatments)!
            .ThenInclude(t => t.TreatmentFormContents)
            .FirstOrDefaultAsync(p => p.Id == patientId && p.DoctorId == doctorId);

        if (patient == null) return false;

        // Remove related TreatmentFormContents
        foreach (var treatment in patient.Treatments!)
        {
            _context.TreatmentFormContents.RemoveRange(treatment.TreatmentFormContents!);
        }

        // Remove related Treatments
        _context.Treatments.RemoveRange(patient.Treatments);

        // Remove the Patient
        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
        return true;
    }


    private async Task<bool> PatientExists(int id) => await _context.Patients.AnyAsync(e => e.Id == id);
}
