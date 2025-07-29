using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.entity.SysModels
{
    public class PageDataOptions
    {
        public int page { get; set; }
        public int size { get; set; }        
        //public string TableName { get; set; }
        public string sort { get; set; }
        /// <summary>
        /// 排序方式
        /// </summary>
        public string? order { get; set; }
        public string? wheres { get; set; }
        public bool? export { get; set; }

        public object value { get; set; }
        /// <summary>
        /// 查询条件
        /// </summary>
        public List<SearchParameters> filter { get; set; }
    }
    public class SearchParameters
    {
        public string name { get; set; }
        public string value { get; set; }
        //查询类型：LinqExpressionType
        public string display_type { get; set; }
    }
}
