using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CRM_Backend.Models.Entities;

namespace CRM_Backend.Data
{
    public class CRMDbContext : IdentityDbContext<Doctor>
    {
        public CRMDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Diagnose> Diagnoses { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<TreatmentForm> TreatmentForms { get; set; }
        public DbSet<TreatmentFormContent> TreatmentFormContents { get; set; }
        public DbSet<MedicineTreatment> MedicineTreatments { get; set; }
        public DbSet<DiagnoseTreatment> DiagnoseTreatments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Doctor>().ToTable("Doctors");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            builder.Entity<Appointment>()
                            .HasOne(a => a.Patient)
                            .WithMany(p => p.Appointments)
                            .HasForeignKey(a => a.PatientId)
                            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Diagnose>()
                .HasOne(d => d.Doctor)
                .WithMany(d => d.Diagnoses)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Medicine>()
                .HasOne(m => m.Doctor)
                .WithMany(d => d.Medicines)
                .HasForeignKey(m => m.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Patient>()
                .HasOne(p => p.Doctor)
                .WithMany(d => d.Patients)
                .HasForeignKey(p => p.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Service>()
                .HasOne(s => s.Doctor)
                .WithMany(d => d.Services)
                .HasForeignKey(s => s.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Treatment>()
                .HasOne(t => t.Service)
                .WithMany(s => s.Treatments)
                .HasForeignKey(t => t.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Treatment>()
                .HasMany(t => t.TreatmentFormContents)
                .WithOne(tf => tf.Treatment)
                .HasForeignKey(tf => tf.TreatmentId)
                .OnDelete(DeleteBehavior.Restrict); // Change this to Restrict

            builder.Entity<TreatmentFormContent>()
                .HasOne(tfc => tfc.Treatment)
                .WithMany(t => t.TreatmentFormContents)
                .HasForeignKey(tfc => tfc.TreatmentId)
                .OnDelete(DeleteBehavior.Restrict); // Change this to Restrict

            builder.Entity<TreatmentFormContent>()
                .HasOne(tfc => tfc.TreatmentForm)
                .WithMany(tf => tf.TreatmentFormContent)
                .HasForeignKey(tfc => tfc.TreatmentFormId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TreatmentForm>()
                .HasOne(tf => tf.Doctor)
                .WithMany(d => d.TreatmentForms)
                .HasForeignKey(tf => tf.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MedicineTreatment>()
                .HasKey(mt => new { mt.MedicineId, mt.TreatmentId });

            builder.Entity<MedicineTreatment>()
                .HasOne(mt => mt.Medicine)
                .WithMany(m => m.MedicineTreatments)
                .HasForeignKey(mt => mt.MedicineId);

            builder.Entity<MedicineTreatment>()
                .HasOne(mt => mt.Treatment)
                .WithMany(t => t.MedicineTreatments)
                .HasForeignKey(mt => mt.TreatmentId);

            builder.Entity<DiagnoseTreatment>()
                .HasKey(dt => new { dt.DiagnoseId, dt.TreatmentId });

            builder.Entity<DiagnoseTreatment>()
                .HasOne(dt => dt.Diagnose)
                .WithMany(d => d.DiagnoseTreatments)
                .HasForeignKey(dt => dt.DiagnoseId);

            builder.Entity<DiagnoseTreatment>()
                .HasOne(dt => dt.Treatment)
                .WithMany(t => t.DiagnoseTreatments)
                .HasForeignKey(dt => dt.TreatmentId);
        }
    }
}
