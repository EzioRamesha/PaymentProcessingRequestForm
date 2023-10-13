using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;
using WebApp.DAL.Helpers;

namespace WebApp.DAL.DAL
{
    internal class DepartmentsAccountDAL : DALBase
    {
        public Guid Save(DepartmentsAccount entity)
        {
            try
            {
                _dbContext.DepartmentsAccounts.Add(entity);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                // Save log
            }
            return entity.Id;
        }

        internal Guid Save(Models.AddDepartmentsAccountViewModel model)
        {
            var returnVal = Guid.Empty;
            try
            {
                var DepartmentsAccount = new Data.DepartmentsAccount
                {
                    Name = model.Name,
                    Code = model.Code,
                    Description= model.Description,
                    PayingEntitiesId=model.PayingEntitiesId,
                    DepartmentId=model.DepartmentId,
                    PayingEntities = model.PayingEntities,
                    Department = model.Department
                };
                _dbContext.DepartmentsAccounts.Add(DepartmentsAccount);
                _dbContext.SaveChanges();
                returnVal = DepartmentsAccount.Id;
            }
            catch (Exception ex)
            {
                returnVal = Guid.Empty;
            }
            return returnVal;
        }

        public Guid Update(Models.UpdateDepartmentsAccountViewModel DepartmentsAccount)
        {
            var returnVal = Guid.Empty;
            try
            {
                var idToSearch = DepartmentsAccount.Id.ToGuid();
                DepartmentsAccount existingDepartmentsAccount = List().Where(w => w.Id.Equals(idToSearch)).FirstOrDefault();
                if (existingDepartmentsAccount != null)
                {
                    existingDepartmentsAccount.Name = DepartmentsAccount.Name;
                    existingDepartmentsAccount.Code = DepartmentsAccount.Code;
                    existingDepartmentsAccount.Description = DepartmentsAccount.Description;
                    existingDepartmentsAccount.PayingEntitiesId = DepartmentsAccount.PayingEntitiesId;
                    existingDepartmentsAccount.DepartmentId = DepartmentsAccount.DepartmentId;
                    existingDepartmentsAccount.Department = DepartmentsAccount.Department;
                    existingDepartmentsAccount.PayingEntities = DepartmentsAccount.PayingEntities;

                    _dbContext.SaveChanges();
                    returnVal = existingDepartmentsAccount.Id;
                }
            }
            catch (Exception ex)
            {

            }
            return returnVal;
        }

        public bool Update(DepartmentsAccount entity)
        {
            try
            {
                DepartmentsAccount oldEntity = _dbContext.DepartmentsAccounts.Where(w => w.Id.Equals(entity.Id)).FirstOrDefault();
                if (oldEntity != null)
                {
                    _dbContext.DepartmentsAccounts.Attach(entity);
                    _dbContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;                   
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
                Data.DepartmentsAccount obj = _dbContext.DepartmentsAccounts.Where(w => w.Id.Equals(id)).FirstOrDefault();
                if (obj != null)
                {
                    obj.IsEnabled = status;
                    _dbContext.SaveChanges();
                    success = true;
                }
            };
            return success;
        }
        public IQueryable<DepartmentsAccount> List()
        {
            return _dbContext.DepartmentsAccounts;
        }
    }
}
