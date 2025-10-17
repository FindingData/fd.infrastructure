using fd.infrastructure.entity;
using fd.infrastructure.entity.SysModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.core.Auth
{
    public interface ITokenGenerator
    {
        TokenResult GenerateToken(UserContext user);
    }
}
