using fd.infrastructure.entity.SysInterface;
using fd.infrastructure.entity.SysModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace fd.infrastructure.core.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<IPage<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query,
           PageRequest request)
        {
            // 排序处理
            if (!string.IsNullOrEmpty(request.order_by))
            {
                var direction = request.order_direction?.ToLower() == "asc" ? "ascending" : "descending";
                query = query.OrderBy($"{request.order_by} {direction}");
            }

            var total = await query.CountAsync();
            var items = await query.Skip((request.page_index - 1) * request.page_size)
                                   .Take(request.page_size)
                                   .ToListAsync();

            return new Paging<T>
            {
                page_no = request.page_index,
                page_size = request.page_size,
                data_count = total,
                data = items
            };
        }
    }
}
