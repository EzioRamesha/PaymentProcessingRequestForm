using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.DAL;
using WebApp.DAL.Models;
using WebApp.DAL.Helpers;
using WebApp.DAL.Models.ResponseModels;

namespace WebApp.DAL.BAL
{
    public class DepartmentBAL
    {
        private static readonly DepartmentDAL _departmentDAL = new DepartmentDAL();

        public Department GetById(string id)
        {
            Guid Id = id.ToGuid();
            return _departmentDAL.List().Where(w => w.Id.Equals(Id)).Select(s => new Department
            {
                Id = s.Id.ToString()
            }).FirstOrDefault();
        }

        public Department GetByName(string name)
        {
            return _departmentDAL.List().Where(w => w.Name.Equals(name)).Select(s => new Department
            {
                Id = s.Id.ToString()
            }).FirstOrDefault();
        }

        public List<Department> GetDepartmentDetails(string PayingEntitiesId)
        {
            return _departmentDAL.List().Where(w => w.PayingEntitiesId.Equals(PayingEntitiesId)).Select(s => new Department
            {
                Id = s.Id.ToString(),
                Name = s.Name
            }).ToList();
        }

        public List<Department> GetAll()
        {
            return _departmentDAL.List().Select(s => new Department
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Code = s.Code,
                Description = s.Description,
                IsEnabled= s.IsEnabled,
                PayingEntitiesId = s.PayingEntitiesId,
                PayingEntities = s.PayingEntities,
                //PayingEntities = s.PayingEntities ==null? s.PayingEntities : string.Empty,

            }).ToList();
        }


        public List<Department> GetActive()
        {
            return _departmentDAL.List().Where(w => w.IsEnabled).Select(s => new Department
            {
                Id = s.Id.ToString(),
                Name = s.Name
            }).ToList();
        }

        public ResponseObject<SaveDepartmentResponse> Save(AddDepartmentViewModel AddDepartment)
        {
            ResponseObject<SaveDepartmentResponse> response;
            using (var _DepartmentDAL = new DepartmentDAL())
            {
                var id = _DepartmentDAL.Save(AddDepartment);
                if (id != Guid.Empty)
                {
                    response = new ResponseObject<SaveDepartmentResponse>
                    {
                        ResponseType = "success",
                        Message = "Department added successfully!"
                    };
                }
                else
                {
                    response = new ResponseObject<SaveDepartmentResponse>
                    {
                        ResponseType = "error",
                        Message = "Something went wrong while adding the Department!"
                    };
                }
            }
            return response;
        }

        public ResponseObject<UpdateDepartmentResponse> Update(UpdateDepartmentViewModel UpdateDepartment)
        {
            ResponseObject<UpdateDepartmentResponse> response;
            using (var _DepartmentDAL = new DepartmentDAL())
            {
                var DepartmentIdToSearch = UpdateDepartment.Id.ToGuid();
                var existingDepartment = _DepartmentDAL.List().Where(w => w.Id.Equals(DepartmentIdToSearch)).FirstOrDefault();

                if (existingDepartment != null)
                {
                    var id = _DepartmentDAL.Update(UpdateDepartment);
                    if (!id.IsNullOrEmpty())
                    {
                        response = new ResponseObject<UpdateDepartmentResponse>
                        {
                            ResponseType = "success",
                            Message = "Successfully updated the Department!"
                        };
                    }
                    else
                    {
                        response = new ResponseObject<UpdateDepartmentResponse>
                        {
                            ResponseType = "error",
                            Message = "Something went wrong while updating the Department!"
                        };
                    }
                }
                else
                {
                    // Currency not found in the database
                    response = new ResponseObject<UpdateDepartmentResponse>
                    {
                        ResponseType = "error",
                        Message = "Department not found!"
                    };
                }

                
            }
            return response;
        }

        public bool Disable(Department Department)
        {
            var isDisabled = false;
            if (Department != null)
            {
                Guid id = Department.Id.ToGuid();
                isDisabled = _departmentDAL.ChangeStatus(id, false);
            }
            return isDisabled;
        }

        public bool Enable(Department Department)
        {
            var isEnabled = false;
            if (Department != null)
            {
                Guid id = Department.Id.ToGuid();
                isEnabled = _departmentDAL.ChangeStatus(id, true);
            }
            return isEnabled;
        }
    }
}
