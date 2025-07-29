using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace fd.infrastructure.entity.SysModels
{
    public class JwtUser
    {
        public int user_id { get; set; }

        public string user_name { get; set; }

        public string display_name { get; set; }

        public string avatar_file { get; set; }

        public string token { get; set; }
    }
}
