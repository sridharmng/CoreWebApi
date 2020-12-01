using CoreWebApi.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.Helpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDTO pagination)
        {
            var result = queryable.Skip((pagination.Page - 1 )* pagination.RecordsPerPage).Take(pagination.RecordsPerPage);
            return result;
        }
    }
}
