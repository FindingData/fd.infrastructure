using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.entity.SysModels
{
    public class Menu
    {
        public string id { get; set; }

        public string name { get; set; }

        public string parent_id { get; set; }

        public string icon { get; set; }

        public int? enable { get; set; }

        public string url { get; set; }

        public string system_flag { get; set; }

        public int sort { get; set; }

    }
}
