using System.Text;
using CRM_Backend.Auth;
using CRM_Backend.Data;
using CRM_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CRM_Backend.Providers;
using CRM_Backend.Services.PatientServices;
using CRM_Backend.Services.DiagnoseServices;
using CRM_Backend.Services.MedicineServices;
using CRM_Backend.Services.ServiceServices;
using CRM_Backend.Services.AppointmentServices;
using CRM_Backend.Services.TreatmentServices;
using CRM_Backend.Services.TreatmentFormServices;
using CRM_Backend.Services.VerificationServices;
using CRM_Backend.Services.StatisticsService;
using CRM_Backend.Services.ProfileServices;

namespace CRM_Backend;

public static class DI
{

    #region Authorization

    public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<Doctor, IdentityRole>(setup => { })
            .AddEntityFrameworkStores<CRMDbContext>()
            .AddDefaultTokenProviders();

        var jwtConfig = new JwtConfig();
        configuration.GetSection("JWT").Bind(jwtConfig);
        services.AddSingleton(jwtConfig);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, setup =>
        {
            setup.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = jwtConfig.Audience,
                ValidIssuer = jwtConfig.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret))
            };
        });

        services.AddAuthorization();

        return services;
    }


    #endregion


    #region DBContext

    public static IServiceCollection AddCRMContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CRMDbContext>(op => op.UseSqlite(configuration.GetConnectionString("Default"))
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
        });

        return services;
    }

    #endregion


    #region Services

    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IDiagnoseService, DiagnoseService>();
        services.AddScoped<IMedicineService, MedicineService>();
        services.AddScoped<IServiceService, ServiceService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<ITreatmentService, TreatmentService>();
        services.AddScoped<ITreatmentFormService, TreatmentFormService>();
        services.AddScoped<IDoctorProvider, DoctorProvider>();
        services.AddScoped<IVerificationCodeService, VerificationCodeService>();
        services.AddScoped<IStatisticsService, StatisticsService>();
        services.AddScoped<IProfileService, ProfileService>();
        return services;
    }

    #endregion

    public static IServiceCollection AddCorsHandle(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins("http://localhost:3000")
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
            });
        });

        return services;
    }



}
