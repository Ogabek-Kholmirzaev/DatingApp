using System.Text.Json;
using API.Helpers;

namespace API.Extentions;

public static class HttpExtensions
{
    public static void AddPaginationHeader<T>(
        this HttpResponse response,
        PagedList<T> data)
        where T : class
    {
        var paginationHeader = new PaginationHeader(
            data.CurrentPage,
            data.PageSize,
            data.TotalCount,
            data.TotalPages);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader, jsonOptions));
        response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
    }
}
