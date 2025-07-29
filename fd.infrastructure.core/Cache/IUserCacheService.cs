using fd.infrastructure.entity.SysModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.core.Cache
{
    public interface IUserCacheService
    {
        UserContext? GetUserContext(int userId);
        void SetUserContext(UserContext context);
        UserContext? LoadUserContextFromDb(int userId);
        Task<UserContext?> GetUserContextAsync(int userId);
        Task SetUserContextAsync(UserContext context);
        Task<UserContext?> LoadUserContextFromDbAsync(int userId);

    }
}
