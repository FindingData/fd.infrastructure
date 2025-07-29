using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.entity.Enums
{
    public enum DbCurrentType
    {
        Default = 0,
        MySql = 1,
        MsSql = 2,//2020.08.08修改sqlserver拼写
        PgSql = 3,
        DM = 4,
        Oracle = 5//2024.03.01
    }
}
