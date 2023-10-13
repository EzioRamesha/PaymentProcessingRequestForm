using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;

namespace WebApp.DAL.DAL
{
    internal class GeneralSettingsDAL : DALBase
    {
        internal IQueryable<ApplicationGroup> ListApplicationGroups()
        {
            return _dbContext.ApplicationGroups;
        }

        internal IQueryable<Permission> ListPermissions()
        {
            return _dbContext.Permissions.Where(w=>w.IsEnabled);
        }

        internal IQueryable<IdentityRole> ListApplicationRoles()
        {
            return _dbContext.Roles;
        }
    }
}
