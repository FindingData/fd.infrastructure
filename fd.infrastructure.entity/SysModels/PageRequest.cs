using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.entity.SysModels
{
    public class PageRequest
    {
        public int page_index { get; set; } = 1;
        public int page_size { get; set; } = 10;

        /// <summary>
        /// 排序字段（如：created_at）
        /// </summary>
        public string? order_by { get; set; }

        /// <summary>
        /// 排序方向：asc / desc（默认 desc）
        /// </summary>
        public string order_direction { get; set; } = "desc";
    }
}
