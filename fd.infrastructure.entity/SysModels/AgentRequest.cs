using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace fd.infrastructure.entity.SysModels
{
    public class AgentRequest
    {
        [Required]
        public string user_input { get; set; }
       
        public string? session_id { get; set;}

        //public JsonNode? data { get; set; }
    }
}
