using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.entity.SysModels
{
    public class OAOptions
    {
        public int customer_id { get; set; }
        public int login_id { get; set; }
        public string login_name { get; set; } = "";
        public string login_pwd { get; set; } = "";

        public List<int> executor_ids { get; set; }
        
        public List<TaskTimeoutRule> TaskTimeoutRules { get; set; }

        public List<TaskDependencyRule> TaskDependencyRules { get; set; }
    }

    public class TaskTimeoutRule
    {
        public int task_type { get; set; }
        public double timeout_hours { get; set; }
        public double warning_hours { get; set; }
        public bool enable_warning { get; set; }
        public bool enable_overdue { get; set; }
        public bool use_llm { get; set; }
    }

    public class TaskDependencyRule
    {
        /// <summary>
        /// 当前任务类型（如查勘）
        /// </summary>
        public int current_task_type { get; set; }

        /// <summary>
        /// 依赖的后续任务类型（如测算）
        /// </summary>
        public int next_required_task_type { get; set; }

        /// <summary>
        /// 当前任务完成后，多少小时内应生成后续任务
        /// </summary>
        public double time_limit_hours { get; set; }

        /// <summary>
        /// 是否使用llm
        /// </summary>
        public bool use_llm { get; set; }

        /// <summary>
        /// 说明（如：查勘完成后必须创建测算）
        /// </summary>
        public string description { get; set; } = string.Empty;

        
    }

}
