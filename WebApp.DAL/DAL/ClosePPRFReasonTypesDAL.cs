using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;

namespace WebApp.DAL.DAL
{
   
    internal class ClosePPRFReasonTypesDAL : DALBase
    {
        public IQueryable<ClosePPRFReasonTypes> List()
        {
            return _dbContext.ClosePPRFReasonType;
        }
        internal void ChangeActiveStatus(Guid guid, bool p)
        {
            try
            {
                ClosePPRFReasonTypes existingEntity = List().Where(w => w.Id.Equals(guid)).FirstOrDefault();
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

        internal Guid Save(ClosePPRFReasonTypes entity)
        {
            Guid returnValue = Guid.Empty;
            try
            {
                var closePPRFReasonTypes = new ClosePPRFReasonTypes
                {
                    Description = entity.Description
                };
                _dbContext.ClosePPRFReasonType.Add(closePPRFReasonTypes);
                _dbContext.SaveChanges();
                returnValue = closePPRFReasonTypes.Id;
            }
            catch (Exception ex)
            {
                // Save log
            }
            return returnValue;
        }


        internal Guid Update(ClosePPRFReasonTypes entity)
        {
            Guid returnId = Guid.Empty;
            var idToSearch = entity.Id;
            var existingEntity = List().Where(w => w.Id.Equals(idToSearch)).FirstOrDefault();
            if (existingEntity != null)
            {

                existingEntity.Description = entity.Description;
                _dbContext.SaveChanges();
                returnId = existingEntity.Id;
            }
            return returnId;
        }

        public List<ClosePPRFReasonTypes> GetRequest()
        {
            var closePPRFReasonTypes = _dbContext.ClosePPRFReasonType.ToList();
            return closePPRFReasonTypes;
        }
    }
}
