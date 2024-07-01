using Microsoft.EntityFrameworkCore;
using TrainCloud.Models;

namespace TrainCloud.Microservices.Core.Extensions.Linq;

public static class PagingExtensions
{
    public static async Task<IQueryable<TRepository>> CalculatePageAsync<TRepository, TItems>(this IQueryable<TRepository> source,
                                                                                              IFilterModel filter,
                                                                                              PageModel<TItems> page,
                                                                                              bool applyOrder = true)
    {
        if (applyOrder)
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

        int repoObjectsCount = await source.CountAsync();
        if (repoObjectsCount > 0)
        {
            page.LastPage = (int)Math.Ceiling((decimal)repoObjectsCount / (decimal)filter.PageSize);
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