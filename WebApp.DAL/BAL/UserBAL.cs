using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.DAL;
using WebApp.DAL.Data;
using WebApp.DAL.Helpers;
using WebApp.DAL.Models;
using WebApp.DAL.Models.ResponseModels;

namespace WebApp.DAL.BAL
{
    public class UserBAL
    {
        //private static readonly UserDAL _userDAL = new UserDAL();
        private static readonly DepartmentDAL _departmentDAL = new DepartmentDAL();
        private static readonly ApplicationUserManager _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(ApplicationDbContext.Create()));
        private readonly GeneralSettingsBAL _generalSettingsBAL = new GeneralSettingsBAL();

        public bool CreateUser(NewUserViewModel model)
        {
            var Id = Guid.Empty;
            try
            {
                using (var _userDAL = new UserDAL())
                {
                    Id = _userDAL.Save(model);
                }
                return !Id.Equals(Guid.Empty);
            }
            catch (Exception ex)
            {
                //throw,
            }
            return false;
        }

        public bool UpdateUser(UpdateUserViewModel model)
        {
            var Id = Guid.Empty;
            try
            {
                using (var _userDAL = new UserDAL())
                {
                    Id = _userDAL.Update(model);
                }
            }
            catch (Exception ex)
            {
                //throw,
            }
            return !Id.Equals(Guid.Empty);
        }


        public ResponseObject<List<Models.User>> GetAllOperators()
        {
            var ResponseObject = new ResponseObject<List<Models.User>>();
            var operators = new List<Models.User>();
            try
            {
                using (var _userDAL = new UserDAL())
                {
                    operators = _userDAL.List().Where(w => !w.ExternalUser.UserName.Equals("admin", StringComparison.OrdinalIgnoreCase)).Select(s => new Models.User
                    {
                        Name = s.Name,
                        Email = s.Email,
                        Department = new Models.Department
                        {
                            Id = s.Department.Id.ToString(),
                            Name = s.Department.Name,
                            Code = s.Department.Code
                        },
                        Designation = s.Designation,
                        IsEnabled = s.IsEnabled
                    }).ToList();
                }
                
                ResponseObject.ResponseType = "success";
                ResponseObject.Data = new List<Models.User>(operators);
            }
            catch (Exception)
            {
                ResponseObject.ResponseType = "error";
                ResponseObject.Message = "Something went wrong while fetching the users";
                //throw;
            }
            return ResponseObject;
        }

        public bool Enable(Models.User user)
        {
            var success = false;
            try
            {
                using (var _userDAL = new UserDAL())
                {
                    _userDAL.ChangeActiveStatus(user, true);
                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }

        public bool EmailAlreadyRegistered(string email)
        {
            var isRegistered = false;
            using (var _userDAL = new UserDAL())
            {
                isRegistered = _userDAL.List().Any(a => a.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            }
            return isRegistered;
        }

        public bool Disable(Models.User user)
        {
            var success = false;
            try
            {
                using (var _userDAL = new UserDAL())
                {
                    _userDAL.ChangeActiveStatus(user, false);
                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }

        public Models.UserDetailsModel FindByEmail(string email)
        {
            UserDetailsModel user = null;
            var activeGroups = _generalSettingsBAL.GetActiveUserGroups();
            using (var _userDAL = new UserDAL())
            {
                var s = _userDAL.List().Where(w => w.Email.Equals(email, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                user = new UserDetailsModel
                {
                    Id = s.Id.ToString(),
                    Name = s.Name,
                    Email = s.Email,
                    Designation = s.Designation,
                    Department = s.Department==null ? null: new Models.Department
                    {
                        Name = s.Department.Name,
                        Id = s.Department.Id.ToString(),
                        Code = s.Department.Code
                    },
                    SelectedGroups = activeGroups.Select(t => new Models.ApplicationGroup
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Selected = s.UserGroups.Any(a => a.ApplicationGroupId.ToString() == t.Id && a.IsSelected)
                    }).ToList(),
                    //SelectedGroups = s.UserGroups.Select(t => new Models.ApplicationGroup
                    //{
                    //    Id = t.ApplicationGroup.Id.ToString(),
                    //    Name = t.ApplicationGroup.Name,
                    //    Selected = t.IsSelected
                    //}).ToList(),
                    Permissions = s.Permissions.Select(p => new Models.UserPermission
                    {
                        Id = p.Id.ToString(),
                        Name = p.Permission.Name,
                        IsEnabled = p.IsEnabled
                    }).ToList()
                };
            }
            return user;
        }

        public List<ApproverGroup> GetApplicationGroups()
        {
            var x = new List<ApproverGroup>();
            using (var _userDAL = new UserDAL())
            {
                x = _userDAL.GetApplicationGroups().Where(w => w.IsEnabled).Select(s => new ApproverGroup
                {
                    GroupName = s.Name,
                    Approvers = s.Users.Where(w1 => !w1.AppUser.Name.Equals("admin") && w1.AppUser.IsEnabled && w1.IsSelected).Select(s2 => new ApproverUser
                    {
                        UserGroupId = s2.Id.ToString(),
                        UserName = s2.AppUser.Name,
                        Email = s2.AppUser.Email,
                        Designation = s2.AppUser.Designation,
                        DepartmentName = s2.AppUser.Department.Name
                    }).ToList()
                }).ToList();
            }
            return x;
        }


        public TokenValidation ValidateToken(string reason, string userEmail, string requestToken, string userToken)
        {
            var result = new TokenValidation();
            byte[] data = Convert.FromBase64String(userToken);
            byte[] _time = data.Take(8).ToArray();
            byte[] _key = data.Skip(8).Take(16).ToArray();
            byte[] _reason = data.Skip(24).Take(reason.Length).ToArray();
            byte[] _Id = data.Skip(reason.Length + 24).Take(userEmail.Length).ToArray();
            byte[] _token = data.Skip(userEmail.Length + reason.Length + 24).ToArray();

            Data.User user = null;
            using (var _userDAL = new UserDAL())
            {
                user = _userDAL.List().Where(w => w.Email.Equals(userEmail, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            }

            DateTime when = DateTime.FromBinary(BitConverter.ToInt64(_time, 0));
            //if (when < DateTime.UtcNow.AddHours(-24))
            //{
            //    result.Errors.Add(TokenValidationStatus.Expired),
            //}

            Guid gKey = new Guid(_key);
            if (gKey.ToString() != user.ExternalUser.SecurityStamp)
            {
                result.Errors.Add(TokenValidationStatus.WrongGuid);
            }

            if (reason != System.Text.Encoding.Default.GetString(_reason))
            {
                result.Errors.Add(TokenValidationStatus.WrongPurpose);
            }

            if (user.Email.ToString() != System.Text.Encoding.Default.GetString(_Id))
            {
                result.Errors.Add(TokenValidationStatus.WrongUser);
            }

            Guid gRequestToken = new Guid(_token);
            if (requestToken != gRequestToken.ToString())
            {
                result.Errors.Add(TokenValidationStatus.WrongUser);
            }

            return result;
        }

        public string ResetPassword(string userEmail)
        {
            try
            {
                var provider = new DpapiDataProtectionProvider("eForms-RRF");
                _userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, string>(provider.Create("UserToken")) as IUserTokenProvider<ApplicationUser, string>;

                var user = _userManager.FindByEmailAsync(userEmail).Result;
                var token = _userManager.GeneratePasswordResetTokenAsync(user.Id).Result;
                var password = System.Web.Security.Membership.GeneratePassword(8,0);
                //Task.Factory.StartNew(() =>
                //{
                //    _userManager.ResetPassword()
                //});
                var result = _userManager.ResetPassword(user.Id, token, password);
                if(result.Succeeded)
                    return password;
                throw new Exception();
            }
            catch(Exception e)
            {
                return e.Message;
            }            
        }

        internal List<WebApp.DAL.Models.UserPermission> GetUserPermissions(string p)
        {
            using (var _userDAL = new UserDAL())
            {
                var user = _userDAL.List().Where(w => w.Email.Equals(p, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (user != null)
                {
                    var permissions = user.Permissions.Select(s => new Models.UserPermission
                    {
                        Id = s.Id.ToString(),
                        Name = s.Permission.Name,
                        IsEnabled = s.IsEnabled
                    }).ToList();
                    return permissions;
                }
            }
            return null;
        }
    }
}
