using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.entity.SysModels
{
    public class FieldFilter
    {
        public string Field { get; set; }
        public string Value { get; set; }

        public string FilterType { get; set; }
    }
}
