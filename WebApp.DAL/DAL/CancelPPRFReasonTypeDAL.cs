using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;
using WebApp.DAL.Helpers;

namespace WebApp.DAL.DAL
{
   

    internal class CancelPPRFReasonTypeDAL : DALBase
    {
        public IQueryable<CancelPPRFReasonTypes> List()
        {
            return _dbContext.CancelPPRFReasonType;
        }
        internal void ChangeActiveStatus(Guid guid, bool p)
        {
            try
            {
                CancelPPRFReasonTypes existingEntity = List().Where(w => w.Id.Equals(guid)).FirstOrDefault();
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

        internal Guid Save(CancelPPRFReasonTypes entity)
        {
            Guid returnValue = Guid.Empty;
            try
            {
                var cancelPPRFReasonTypes = new CancelPPRFReasonTypes
                {
                    Description = entity.Description
                };
                _dbContext.CancelPPRFReasonType.Add(cancelPPRFReasonTypes);
                _dbContext.SaveChanges();
                returnValue = cancelPPRFReasonTypes.Id;
            }
            catch (Exception ex)
            {
                // Save log
            }
            return returnValue;
        }


        internal Guid Update(WebApp.DAL.Data.CancelPPRFReasonTypes entity)
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

        public List<WebApp.DAL.Data.CancelPPRFReasonTypes> GetRequest()
        {
            var cancelPPRFReasonTypes = _dbContext.CancelPPRFReasonType.ToList();
            return cancelPPRFReasonTypes;
        }
    }
}
