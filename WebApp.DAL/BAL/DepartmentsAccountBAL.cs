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
   
    public class DepartmentsAccountBAL
    {
        private static readonly DAL.DepartmentsAccountDAL _departmentsAccount = new DAL.DepartmentsAccountDAL();

        public DepartmentsAccount GetById(string id)
        {
            Guid Id = id.ToGuid();
            return _departmentsAccount.List().Where(w => w.Id.Equals(Id)).Select(s => new DepartmentsAccount
            {
                Id = s.Id.ToString()
            }).FirstOrDefault();
        }
        public DepartmentsAccount GetByName(string name)
        {
            return _departmentsAccount.List().Where(w => w.Name.Equals(name)).Select(s => new DepartmentsAccount
            {
                Id = s.Id.ToString()
            }).FirstOrDefault();
        }
        public List<DepartmentsAccount> GetAll()
        {
            return _departmentsAccount.List().Select(s => new DepartmentsAccount
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Code=s.Code,
                Description=s.Description,
                PayingEntitiesId=s.PayingEntitiesId,
                IsEnabled = s.IsEnabled,
                DepartmentId =s.DepartmentId,
                PayingEntities = s.PayingEntities,
                Department = s.Department
            }).ToList();
        }

        public List<DepartmentsAccount> GetDepartmentsAccountDetails(DepartmentsAccount DepartmentId)
        {
            string strid = DepartmentId.Id;
            return _departmentsAccount.List().Where(w=> w.DepartmentId== strid).Select(s => new DepartmentsAccount
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Code = s.Code,
                Description = s.Description,
                PayingEntitiesId = s.PayingEntitiesId,
                DepartmentId = s.DepartmentId,
                PayingEntities = s.PayingEntities,
                Department = s.Department
            }).ToList();
        }


        public List<DepartmentsAccount> GetActive()
        {
            return _departmentsAccount.List().Where(w => w.IsEnabled).Select(s => new DepartmentsAccount
            {
                Id = s.Id.ToString(),
                Name = s.Name
            }).ToList();
        }

        public ResponseObject<SaveDepartmentsAccountResponse> Save(AddDepartmentsAccountViewModel AddDepartmentsAccount)
        {
            ResponseObject<SaveDepartmentsAccountResponse> response;
            using (var _departmentsAccount = new DepartmentsAccountDAL())
            {
                var id = _departmentsAccount.Save(AddDepartmentsAccount);
                if (id != Guid.Empty)
                {
                    response = new ResponseObject<SaveDepartmentsAccountResponse>
                    {
                        ResponseType = "success",
                        Message = "Department added successfully!"
                    };
                }
                else
                {
                    response = new ResponseObject<SaveDepartmentsAccountResponse>
                    {
                        ResponseType = "error",
                        Message = "Something went wrong while adding the Department!"
                    };
                }
            }
            return response;
        }

        public ResponseObject<UpdateDepartmentsAccountResponse> Update(UpdateDepartmentsAccountViewModel UpdateDepartment)
        {
            ResponseObject<UpdateDepartmentsAccountResponse> response;
            using (var _departmentsAccount = new DepartmentsAccountDAL())
            {
                var DepartmentIdToSearch = UpdateDepartment.Id.ToGuid();
                var existingDepartment = _departmentsAccount.List().Where(w => w.Id.Equals(DepartmentIdToSearch)).FirstOrDefault();

                if (existingDepartment != null)
                {
                    var id = _departmentsAccount.Update(UpdateDepartment);
                    if (!id.IsNullOrEmpty())
                    {
                        response = new ResponseObject<UpdateDepartmentsAccountResponse>
                        {
                            ResponseType = "success",
                            Message = "Successfully updated the Department!"
                        };
                    }
                    else
                    {
                        response = new ResponseObject<UpdateDepartmentsAccountResponse>
                        {
                            ResponseType = "error",
                            Message = "Something went wrong while updating the Department!"
                        };
                    }
                }
                else
                {
                    // Currency not found in the database
                    response = new ResponseObject<UpdateDepartmentsAccountResponse>
                    {
                        ResponseType = "error",
                        Message = "Department not found!"
                    };
                }


            }
            return response;
        }

        public bool Disable(DepartmentsAccount DepartmentsAccount)
        {
            var isDisabled = false;
            if (DepartmentsAccount != null)
            {
                Guid id = DepartmentsAccount.Id.ToGuid();
                isDisabled = _departmentsAccount.ChangeStatus(id, false);
            }
            return isDisabled;
        }

        public bool Enable(DepartmentsAccount DepartmentsAccount)
        {
            var isEnabled = false;
            if (DepartmentsAccount != null)
            {
                Guid id = DepartmentsAccount.Id.ToGuid();
                isEnabled = _departmentsAccount.ChangeStatus(id, true);
            }
            return isEnabled;
        }
    }
}
