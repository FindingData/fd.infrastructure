using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.entity.SysInterface
{
    public interface IPage<T>
    {
        /// <summary>
        /// All data rows
        /// </summary>
        int data_count { get; set; }

        /// <summary>
        ///pageCount
        /// </summary>
        int page_count { get; }

        /// <summary>
        /// pageNo
        /// </summary>
        int page_no { get; set; }

        /// <summary>
        /// pageSize
        /// </summary>
        int page_size { get; set; }

        /// <summary>
        /// data
        /// </summary>
        List<T> data { get; set; }


        //dynamic summary { get; set; }
    }
}
