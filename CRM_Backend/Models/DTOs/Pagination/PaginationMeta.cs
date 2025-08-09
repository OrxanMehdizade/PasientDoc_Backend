namespace CRM_Backend.Models.DTOs.Pagination;

public class PaginationMeta(int page, int pageSize, int itemCount)
{
    public int Page { get; set; } = page;
    public int PageSize { get; set; } = pageSize;
    public int TotalPages { get; set; } = (itemCount + pageSize - 1) / pageSize;
}
