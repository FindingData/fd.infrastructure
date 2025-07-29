using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.attendai.domain.Model
{
    public class TaskTimeoutRule
    {
        public int task_type { get; set; }
        public double timeout_hours { get; set; }
        public double warning_hours { get; set; }
        public bool enable_warning { get; set; }
        public bool enable_overdue { get; set; }
        public bool use_llm { get; set; }
    }

}
