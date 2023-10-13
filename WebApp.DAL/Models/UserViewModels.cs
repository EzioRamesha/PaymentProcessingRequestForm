using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Models
{
    public class NewUserViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public Department Department { get; set; }

        public string ExternalUserId { get; set; }

        [Required]
        public string Designation { get; set; }

        public AppRole Role { get; set; }

        public IEnumerable<ApplicationGroup> SelectedGroups { get; set; }

        public IEnumerable<Permission> Permissions { get; set; }
    }


    public class UpdateUserViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }


        public Department Department { get; set; }
        public string Designation { get; set; }

        public IEnumerable<ApplicationGroup> SelectedGroups { get; set; }

        public IEnumerable<Permission> Permissions { get; set; }
    }


    public class User
    {
        internal string Id { get; set; }
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public Department Department { get; set; }
        //public string Department { get; set; }
        public string Designation { get; set; }

        public bool IsEnabled { get; set; }
        
    }

    public class UserDetailsModel : User
    {
        public List<ApplicationGroup> SelectedGroups { get; set; }
        public List<UserPermission> Permissions { get; set; }
    }

    public class ApproverUser
    {
        public string UserGroupId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Designation { get; set; }
        public string DepartmentName { get; set; }
        public int SequenceNo { get; set; }
        public bool Selected { get; set; }
        public ApproverUser()
        {
            Selected = false;
        }
    }
    public class ApproverGroup
    {
        public string GroupName { get; set; }
        public List<ApproverUser> Approvers { get; set; }
    }


    public class ApplicationGroup
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
    }


    public class Permission
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
    }
    public class UserPermission
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class UpdateUserPermissionsModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }


        public Department Department { get; set; }
        public string Designation { get; set; }

        public IEnumerable<UserPermission> Permissions { get; set; }
    }
}
