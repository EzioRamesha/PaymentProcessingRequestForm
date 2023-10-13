using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;
using WebApp.DAL.Helpers;

namespace WebApp.DAL.DAL
{
    internal class FrequencyTypeDAL : DALBase
    {
        public IQueryable<FrequencyType> List()
        {
            return _dbContext.FrequencyTypes;
        }

        internal void ChangeActiveStatus(Guid guid, bool p)
        {
            try
            {
                FrequencyType existingEntity = List().Where(w => w.Id.Equals(guid)).FirstOrDefault();
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

        internal Guid Save(Models.FrequencyType entity)
        {
            Guid returnValue = Guid.Empty;
            try
            {
                var frequencyType = new FrequencyType
                {
                    Name = entity.Name,
                    Description = entity.Description
                };
                _dbContext.FrequencyTypes.Add(frequencyType);
                _dbContext.SaveChanges();
                returnValue = frequencyType.Id;
            }
            catch (Exception ex)
            {
                // Save log
            }
            return returnValue;
        }


        internal Guid Update(Models.FrequencyType entity)
        {
            Guid returnId = Guid.Empty;
            var idToSearch = entity.Id.ToGuid();
            var existingEntity = List().Where(w => w.Id.Equals(idToSearch)).FirstOrDefault();
            if (existingEntity != null)
            {
                existingEntity.Name = entity.Name;
                existingEntity.Description = entity.Description;
                _dbContext.SaveChanges();
                returnId = existingEntity.Id;
            }
            return returnId;
        }
    }
}
