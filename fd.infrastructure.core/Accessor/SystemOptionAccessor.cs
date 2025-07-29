using fd.infrastructure.entity.SysModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.core.Accessor
{
    public static class SystemOptionAccessor
    {
        private static SystemOptions _options;

        public static void SetOptions(SystemOptions options)
        {
            _options = options;
        }

        public static SystemOptions Value => _options;
    }

}
