using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.entity.SysModels
{
    public class UserContext
    {
        public int user_id
        {
            get; set;
        }
        
        public int? customer_id
        {
            get; set;
        }

        public string? login_id
        {
            get; set;
        }

        public string? user_name
        {
            get; set;
        }

        public string? display_name
        {
            get; set;
        }

        public string? user_type
        {
            get; set;
        }

        public string? phone
        {
            get; set;
        }

        public string? avatar_file
        {
            get; set;
        }

        public string? token { get; set; }

        public string? getui_client_id
        {
            get; set;
        }

        public string? ip_address { get; set; }

        public string? device_type { get; set; }

        public string? platform { get; set; }               
        public IList<string> resource_keys { get; set; }

        public IList<Permission> permissions { get; set; } = new List<Permission>();

    }
}
