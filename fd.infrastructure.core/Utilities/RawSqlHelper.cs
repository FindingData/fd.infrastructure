using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.core.Utilities
{
    public static class RawSqlHelper
    {
        public static string RawInClause(IEnumerable<long> ids)
        {
            return string.Join(",", ids.Select(id => id.ToString()));
        }
    }
}
