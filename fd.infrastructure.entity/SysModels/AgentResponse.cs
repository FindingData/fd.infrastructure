using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.entity.SysModels
{
    public class AgentResponse<T>
    {
        /// <summary>
        /// 业务状态："complete"|"incomplete"|"error" 等
        /// </summary>
        public string status { get; set; }

        public string message { get; set; } = "操作成功";

        public T data { get; set; }

        /// <summary>
        /// 多轮补全下次交互提示，仅需要补全时有值
        /// </summary>
        public string next_prompt { get; set; }

    }
}
