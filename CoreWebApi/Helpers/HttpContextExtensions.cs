using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;   
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.Helpers
{
    public static class HttpContextExtensions
    {
        public static async Task InsertPaginationParametersInRespnse<T>(this HttpContext httpContext,
            IQueryable<T> queryable, int recordsPerPage)
        {
            if (httpContext == null) { throw new ArgumentNullException(nameof(httpContext)); }
            double count = await queryable.CountAsync();
            double totalNumberOfPages = Math.Ceiling(recordsPerPage / count);
            httpContext.Response.Headers.Add("TotalNumberofPages", totalNumberOfPages.ToString());
        }
    }
}
