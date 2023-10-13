using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;
using WebApp.DAL.Helpers;

namespace WebApp.DAL.DAL
{
    internal class UserDAL : DALBase
    {

        public Guid Save(Models.NewUserViewModel model)
        {
            Guid returnVal = Guid.Empty;
            //Console.WriteLine("Begin");
            try
            {
                var user = new Data.User
                {
                    Name = model.Name,
                    DepartmentId = model.Department.Id.ToGuid(),
                    Designation = model.Designation,
                    CreatedOn = GeneralHelper.CurrentDate(),
                    ExternalUserId = model.ExternalUserId,
                    Email = model.Email
                };
                var existingUser = List().Where(w => w.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (existingUser == null)
                {
                    //Console.WriteLine("Create New User");
                    //user.UserGroups = new List<ApplicationUserGroup>();
                    var applicationGroups = _dbContext.ApplicationGroups.Where(w => w.IsEnabled).ToList();
                    //Console.WriteLine("Application Groups: " + applicationGroups.Count);
                    applicationGroups.ForEach(f =>
                    {
                        user.UserGroups.Add(new ApplicationUserGroup
                        {
                            ApplicationGroup = f,
                            AppUser = user,
                            IsSelected = model.SelectedGroups.Where(w => w.Id.Equals(f.Id.ToString())).FirstOrDefault() == null ? false :
                                                        model.SelectedGroups.Where(w => w.Id.Equals(f.Id.ToString())).FirstOrDefault().Selected
                        });
                    });
                    //Console.WriteLine("Added application groups");
                    var permissions = _dbContext.Permissions.Where(w => w.IsEnabled).ToList();
                    //Console.WriteLine("Permissions: " + permissions.Count);
                    permissions.ForEach(permission =>
                    {
                        user.Permissions.Add(new UserPermission
                        {
                            User = user,
                            Permission = permission,
                            IsEnabled = permission.IsEnabled
                        });
                    });
                    //Console.WriteLine("Added permissions");
                    _dbContext.AppUsers.Add(user);
                    _dbContext.SaveChanges();
                    //Console.WriteLine("User Added");
                    returnVal = user.Id;
                }
            }
            catch (Exception ex)
            {
                //throw;
            }
            //Console.ReadLine();
            return returnVal;
        }


        public Guid UpdatePermissions(Models.UpdateUserPermissionsModel model)
        {
            try
            {
                User existingUser = _dbContext.AppUsers.Where(w => w.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (existingUser != null)
                {
                    model.Permissions.ToList().ForEach(permission => {
                        var oldPermission = existingUser.Permissions.Where(w => w.PermissionId.Equals(permission.Id.ToGuid())).FirstOrDefault();
                        if (oldPermission != null)
                        {
                            oldPermission.IsEnabled = permission.IsEnabled;
                        }
                        else
                        {
                            existingUser.Permissions.Add(new UserPermission { 
                                PermissionId = permission.Id.ToGuid(),
                                UserId = existingUser.Id,
                                IsEnabled = permission.IsEnabled
                            });
                        }
                    });
                    _dbContext.SaveChanges();
                    return existingUser.Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Guid.Empty;
        }


        public Guid Update(Models.UpdateUserViewModel model)
        {
            try
            {
                User existingUser = _dbContext.AppUsers.Where(w => w.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (existingUser != null)
                {
                    existingUser.Name = model.Name;
                    Guid? departmentId = model.Department == null ? null : (Guid?)model.Department.Id.ToGuid();
                    existingUser.Department = _dbContext.Departments.Where(w => w.Id == departmentId).FirstOrDefault();
                    existingUser.Designation = model.Designation;


                    model.SelectedGroups.ToList().ForEach(f =>
                    {
                        var assignedGroup = existingUser.UserGroups.Where(w => w.ApplicationGroupId.Equals(f.Id.ToGuid())).FirstOrDefault();
                        if (assignedGroup != null)
                        {
                            assignedGroup.IsSelected = f.Selected;
                        }
                        else
                        {
                            existingUser.UserGroups.Add(new ApplicationUserGroup
                            {
                                UserId = existingUser.Id,
                                ApplicationGroupId = f.Id.ToGuid(),
                                IsSelected = f.Selected
                            });
                        }
                    });
                    model.Permissions.ToList().ForEach(permission =>
                    {
                        var oldPermission = existingUser.Permissions.Where(w => w.Id.Equals(permission.Id.ToGuid())).FirstOrDefault();
                        if (oldPermission != null)
                        {
                            oldPermission.IsEnabled = permission.IsEnabled;
                        }
                        else
                        {
                            existingUser.Permissions.Add(new UserPermission
                            {
                                PermissionId = permission.Id.ToGuid(),
                                UserId = existingUser.Id,
                                IsEnabled = permission.IsEnabled
                            });
                        }
                    });
                    _dbContext.SaveChanges();
                    return existingUser.Id;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Guid.Empty;
        }

        internal void ChangeActiveStatus(Models.User user, bool status)
        {
            try
            {
                User existingUser = List().Where(w => w.Email.Equals(user.Email)).FirstOrDefault();
                if (existingUser != null)
                {
                    existingUser.IsEnabled = status;
                }
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public IQueryable<Data.User> List()
        {
            return _dbContext.AppUsers;
        }


        public IQueryable<Data.ApplicationGroup> GetApplicationGroups()
        {
            return _dbContext.ApplicationGroups;
        }


        public User FindByEmail(string email)
        {
            User user = null;
            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                user = dbContext.AppUsers.Where(w => w.Email.Equals(email)).FirstOrDefault();
            }
            return user;
        }
    }
}
