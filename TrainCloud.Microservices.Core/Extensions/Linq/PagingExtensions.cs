using Microsoft.EntityFrameworkCore;
using TrainCloud.Models;

namespace TrainCloud.Microservices.Core.Extensions.Linq;

/// <summary>
/// Extensions for paging in api objects (e.g.: Cars/Owners/CarLists/...)
/// </summary>
public static class PagingExtensions
{
    /// <summary>
    /// Calculates a page for a EF Core repo with the given parameters.
    /// </summary>
    /// <typeparam name="TRepository">The type of the DbSet in the EF Core repo</typeparam>
    /// <typeparam name="TItems">The type of the Model items in page.Items</typeparam>
    /// <param name="source">EF Core repo to page.</param>
    /// <param name="filter">Page settings (page size/ items per page)</param>
    /// <param name="page">The page object to be filled</param>
    /// <param name="orderItems">If true, the page gets ordered by the specified field in filter.OrderBy. 
    ///                          If false, the data have to be ordered before calculating the page.</param>
    /// <returns></returns>
    public static async Task<IQueryable<TRepository>> CalculatePageAsync<TRepository, TItems>(this IQueryable<TRepository> source,
                                                                                              IFilterModel filter,
                                                                                              PageModel<TItems> page,
                                                                                              bool orderItems = true)
    {
        if (orderItems)
        {
            switch (filter.Order)
            {
                case SortOrder.Ascending:
                    source = source.OrderBy($"{filter.OrderBy}");
                    break;
                case SortOrder.Descending:
                    source = source.OrderByDescending($"{filter.OrderBy}");
                    break;
            }
        }

        if (filter.PageNr < 1)
        {
            filter.PageNr = 1;
        }

        page.PageSize = filter.PageSize;

        page.TotalCount = await source.CountAsync();
        if (page.TotalCount > 0)
        {
            page.LastPage = (int)Math.Ceiling((decimal)page.TotalCount / (decimal)filter.PageSize);
        }
        else
        {
            page.LastPage = 1;
        }

        if (filter.PageNr > page.LastPage)
        {
            filter.PageNr = page.LastPage;
            page.CurrentPage = filter.PageNr;
        }
        else
        {
            page.CurrentPage = filter.PageNr;
        }

        int skip = (filter.PageNr - 1) * filter.PageSize;

        source = source.Skip(skip).Take(filter.PageSize);

        return source;
    }
}