using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.entity.SysModels
{
    public class SystemOptions
    {
        /// <summary>
        /// 系统唯一标识ID（如 attendai 可为 1）
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// 系统编码（如 attendai / reportai）
        /// </summary>
        public string SystemCode { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { get; set; }
    }

}
