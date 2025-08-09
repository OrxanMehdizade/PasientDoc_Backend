using CRM_Backend.Models.DTOs.Statistics;

namespace CRM_Backend.Services.StatisticsService;

public interface IStatisticsService
{
    Task<GeneralStatisticsDto> GetGeneralStatisticsAsync(StatisticsFilterDto filter);
    Task<AppealStatisticsDto> GetPatientsComeFromAsync(StatisticsFilterDto filter);
}
