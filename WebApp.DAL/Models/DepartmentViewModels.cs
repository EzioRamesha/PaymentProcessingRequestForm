using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Models
{
    public class AddDepartmentViewModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string PayingEntitiesId { get; set; }
        public string PayingEntities { get; set; }
        public string Description { get; set; }
    }

    public class UpdateDepartmentViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string PayingEntitiesId { get; set; }
        public string Description { get; set; }
        public string PayingEntities { get; set; }
    }

    public class Department
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public string PayingEntitiesId { get; set; }
        public string PayingEntities { get; set; }
    }

    public class ManageDepartmentsViewModel
    {
        public List<Department> Departments { get; set; }
    }
}
