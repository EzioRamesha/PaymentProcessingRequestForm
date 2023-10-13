using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;
using WebApp.DAL.Helpers;

namespace WebApp.DAL.DAL
{
   
    internal class RejectTypesDAL : DALBase
    {
        public IQueryable<RejectTypes> List()
        {
            return _dbContext.RejectTypes;
        }
        internal void ChangeActiveStatus(Guid guid, bool p)
        {
            try
            {
                RejectTypes existingEntity = List().Where(w => w.Id.Equals(guid)).FirstOrDefault();
                if (existingEntity != null)
                {
                    existingEntity.IsEnabled = p;
                }
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal Guid Save(Models.RejectType entity)
        {
            Guid returnValue = Guid.Empty;
            try
            {
                var rejectTypes = new RejectTypes
                {                   
                    Description = entity.Description
                };
                _dbContext.RejectTypes.Add(rejectTypes);
                _dbContext.SaveChanges();
                returnValue = rejectTypes.Id;
            }
            catch (Exception ex)
            {
                // Save log
            }
            return returnValue;
        }


        internal Guid Update(Models.RejectType entity)
        {
            Guid returnId = Guid.Empty;
            var idToSearch = entity.Id.ToGuid();
            var existingEntity = List().Where(w => w.Id.Equals(idToSearch)).FirstOrDefault();
            if (existingEntity != null)
            {
              
                existingEntity.Description = entity.Description;
                _dbContext.SaveChanges();
                returnId = existingEntity.Id;
            }
            return returnId;
        }

        public List<WebApp.DAL.Data.RejectTypes> GetRequest()
        {
            var rejectsType = _dbContext.RejectTypes.ToList();
            return rejectsType;
        }
    }
}
