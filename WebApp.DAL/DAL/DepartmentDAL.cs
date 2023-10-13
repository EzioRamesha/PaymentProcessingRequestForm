using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;
using WebApp.DAL.Helpers;

namespace WebApp.DAL.DAL
{
    internal class DepartmentDAL : DALBase
    {
        //public Guid Save(Department entity)
        //{
        //    try
        //    {
        //        _dbContext.Departments.Add(entity);
        //        _dbContext.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Save log
        //    }
        //    return entity.Id;
        //}

        internal Guid Save(Models.AddDepartmentViewModel model)
        {
            var returnVal = Guid.Empty;
            try
            {
                var Department = new Data.Department
                {
                    Name = model.Name,
                    Code = model.Code,
                    PayingEntitiesId = model.PayingEntitiesId,
                    Description=model.Description,
                    PayingEntities = model.PayingEntities
            };
                _dbContext.Departments.Add(Department);
                _dbContext.SaveChanges();
                returnVal = Department.Id;
            }
            catch (Exception ex)
            {
                returnVal = Guid.Empty;
            }
            return returnVal;
        }
        public Guid Update(Models.UpdateDepartmentViewModel Department)
        {
            var returnVal = Guid.Empty;
            try
            {
                var idToSearch = Department.Id.ToGuid();
                Department existingDepartment = List().Where(w => w.Id.Equals(idToSearch)).FirstOrDefault();
                if (existingDepartment != null)
                {
                    existingDepartment.Name = Department.Name;
                    existingDepartment.Code = Department.Code;
                    if(!string.IsNullOrEmpty(Department.PayingEntitiesId))
                    {
                        existingDepartment.PayingEntitiesId = Department.PayingEntitiesId;
                        existingDepartment.PayingEntities = Department.PayingEntities;
                    }                    
                    existingDepartment.Description = Department.Description;
                    _dbContext.SaveChanges();
                    returnVal = existingDepartment.Id;
                }
            }
            catch (Exception ex)
            {

            }
            return returnVal;
        }

        public bool Update(Department entity)
        {
            try
            {
                Department oldEntity = _dbContext.Departments.Where(w => w.Id.Equals(entity.Id)).FirstOrDefault();
                if (oldEntity != null)
                {
                    _dbContext.Departments.Attach(entity);
                    _dbContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    //_dbContext.Departments.Add(entity);
                    _dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Save log
            }
            return false;
        }
        internal bool ChangeStatus(Guid id, bool status)
        {
            bool success = false;
            if (!id.Equals(Guid.Empty))
            {
                Data.Department obj = _dbContext.Departments.Where(w => w.Id.Equals(id)).FirstOrDefault();
                if (obj != null)
                {
                    obj.IsEnabled = status;
                    _dbContext.SaveChanges();
                    success = true;
                }
            };
            return success;
        }
        public IQueryable<Department> List()
        {
            return _dbContext.Departments;
        }
        public List<Department> GetAllDepartment()
        {
            var ReturnDepartments = _dbContext.Departments.ToList();
            return ReturnDepartments;
        }
    }
}
