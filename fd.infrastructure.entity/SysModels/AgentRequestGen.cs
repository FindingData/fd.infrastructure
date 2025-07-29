using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.entity.SysModels
{
    public class AgentRequest<T>
    {
        [Required]
        public string user_input { get; set; }

        public T? Payload { get; set; }
    }
}
