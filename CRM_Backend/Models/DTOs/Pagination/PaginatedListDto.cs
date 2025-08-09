namespace CRM_Backend.Models.DTOs.Pagination;

public class PaginatedListDto<TModel>(IEnumerable<TModel> items, PaginationMeta meta)
{
    public IEnumerable<TModel> Items { get; set; } = items;
    public PaginationMeta Meta { get; set; } = meta;
}
