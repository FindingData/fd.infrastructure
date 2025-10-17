using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.entity
{
    public sealed class TokenResult
    {
        public string token { get; init; } = "";
        public long expires_sec { get; init; }
        public string jti { get; init; } = "";
    }
}
