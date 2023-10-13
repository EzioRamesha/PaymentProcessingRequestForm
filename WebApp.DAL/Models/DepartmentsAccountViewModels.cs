using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Models
{
    public class AddDepartmentsAccountViewModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string PayingEntitiesId { get; set; }
        public string DepartmentId { get; set; }
        public string Department { get; set; }
        public string PayingEntities { get; set; }
    }

    public class UpdateDepartmentsAccountViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string PayingEntitiesId { get; set; }
        public string DepartmentId { get; set; }
        public string Department { get; set; }
        public string PayingEntities { get; set; }
    }
    public class DepartmentsAccount
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string PayingEntitiesId { get; set; }
        public bool IsEnabled { get; set; }
        public string DepartmentId { get; set; }
        public string Department { get; set; }
        public string PayingEntities { get; set; }
    }
    public class ManageDepartmentsAccountViewModel
    {
        public List<Department> Departments { get; set; }
    }
   
}
