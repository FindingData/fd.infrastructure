using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.entity.SysModels
{
    public class Permission
    {
        public string id { get; set; }

        public string parent_id { get; set; }

        public string? name { get; set; }
        
    }
}
