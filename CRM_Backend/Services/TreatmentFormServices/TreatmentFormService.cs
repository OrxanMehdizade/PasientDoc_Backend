using CRM_Backend.Data;
using CRM_Backend.Models.DTOs.TreatmentForm;
using CRM_Backend.Models.Entities;
using CRM_Backend.Providers;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Services.TreatmentFormServices
{
    public class TreatmentFormService(CRMDbContext context, IDoctorProvider doctorProvider) : ITreatmentFormService
    {
        private readonly CRMDbContext _context = context;
        private readonly IDoctorProvider _doctorProvider = doctorProvider;

        public async Task<List<GetTreatFormsDto>> GetTreatmentFormsAsync()
        {
            var doctorId = _doctorProvider.GetDoctorId();
            var treatmentForms = await _context.TreatmentForms
                .Where(tf => tf.DoctorId == doctorId)
                .Select(tf => new GetTreatFormsDto { Id = tf.Id, Name = tf.Name })
                .Distinct()
                .ToListAsync();

            return treatmentForms;
        }

        public async Task<TreatmentForm> GetTreatmentFormByIdAsync(int id)
        {
            var doctorId = _doctorProvider.GetDoctorId();
            return await _context.TreatmentForms
                .Where(tf => tf.Id == id && tf.DoctorId == doctorId)
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateTreatmentFormAsync(CreateTreatmentFormRequest request)
        {
            try
            {
                var doctorId = _doctorProvider.GetDoctorId();
                var treatmentForm = new TreatmentForm
                {
                    DoctorId = doctorId,
                    Name = request.Name,
                };
                _context.TreatmentForms.Add(treatmentForm);
                await _context.SaveChangesAsync();
                return treatmentForm.Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<bool> UpdateTreatmentFormAsync(int id, UpdateTreatmentFormRequest treatmentForm)
        {
            var doctorId = _doctorProvider.GetDoctorId();

            var existingTreatmentForm = await _context.TreatmentForms
                .FirstOrDefaultAsync(tf => tf.Id == id && tf.DoctorId == doctorId);

            if (existingTreatmentForm == null) return false;

            existingTreatmentForm.Name = treatmentForm.Name;

            _context.Entry(existingTreatmentForm).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TreatmentFormExists(id)) return false;
                else throw;
            }
        }

        public async Task<bool> DeleteTreatmentFormAsync(int id)
        {
            var doctorId = _doctorProvider.GetDoctorId();
            var treatmentForm = await _context.TreatmentForms
                .Where(tf => tf.Id == id && tf.DoctorId == doctorId)
                .FirstOrDefaultAsync();

            if (treatmentForm == null) return false;

            _context.TreatmentForms.Remove(treatmentForm);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> TreatmentFormExists(int id) => await _context.TreatmentForms.AnyAsync(e => e.Id == id);
    }
}
