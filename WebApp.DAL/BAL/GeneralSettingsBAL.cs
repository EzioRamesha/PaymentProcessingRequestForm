using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.DAL;
using WebApp.DAL.Models;

namespace WebApp.DAL.BAL
{
    public class GeneralSettingsBAL
    {
        //private static readonly GeneralSettingsDAL _generalSettingsDAL = new GeneralSettingsDAL();
        public List<ApplicationGroup> GetActiveUserGroups()
        {
            using (var _generalSettingsDAL = new GeneralSettingsDAL())
            {
                return _generalSettingsDAL.ListApplicationGroups().Where(w => w.IsEnabled).Select(s => new ApplicationGroup
                {
                    Id = s.Id.ToString(),
                    Name = s.Name
                }).ToList();
            }
        }

        public List<Permission> GetActiveUserPermissions()
        {
            using (var _generalSettingsDAL = new GeneralSettingsDAL())
            {
                return _generalSettingsDAL.ListPermissions().Where(w => w.IsEnabled).Select(s => new Permission
                {
                    Id = s.Id.ToString(),
                    Name = s.Name
                }).ToList();
            }
        }

        public List<AppRole> GetAllRolesExceptAdmin()
        {
            using (var _generalSettingsDAL = new GeneralSettingsDAL())
            {
                return _generalSettingsDAL.ListApplicationRoles().Where(w => !w.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase)).Select(s => new AppRole
                {
                    Id = s.Id.ToString(),
                    Name = s.Name
                }).ToList();
            }
        }
    }
}
