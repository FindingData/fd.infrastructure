using fd.infrastructure.entity.SysInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.entity.SysModels
{
    public class Paging<T> : IPage<T>
    {
        /// <summary>
        /// All data rows
        /// </summary>
        public int data_count { get; set; }
        /// <summary>
        /// 总页数|pageCount
        /// </summary>
        public int page_count => (int)System.Math.Ceiling((decimal)this.data_count / page_size);

        /// <summary>
        /// 当前页码|page number
        /// </summary>
        public int page_no { get; set; }

        /// <summary>
        /// 每页显示记录数|page size
        /// </summary>
        public int page_size { get; set; } = 1;



        /// <summary>
        /// 当前分页数据|data list
        /// </summary>
        public List<T> data { get; set; }

        //public dynamic summary { get; set; }

        /// <summary>
        /// 分页数据|
        /// paging data
        /// </summary>
        public Paging()
        {
            data = new List<T>();
        }

        /// <summary>
        /// 分页数据|
        /// paging data
        /// </summary>
        /// <param name="pageNo">page number</param>
        /// <param name="pageSize">page size</param>
        public Paging(int pageNo, int pageSize) : this()
        {
            this.page_no = pageNo;
            this.page_size = pageSize;
        }
    }
}
