using fd.infrastructure.entity.SysModels;
using fd.infrastructure.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.core.Extensions
{
    public static class ResponseExtensions
    {
        /// <summary>
        /// 将 WebResponseContent 转换为 AgentResponse
        /// </summary>
        public static AgentResponse<T> ToAgentResponse<T>(this WebResponseContent webResponse)
        {
            return new AgentResponse<T>
            {
                status = "complete",
                message = webResponse.message,
                data = webResponse.data is T val ? val : default
            };
        }
    }
}
