using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;
using WebApp.DAL.Helpers;

namespace WebApp.DAL.DAL
{
    internal class PayingEntityDAL : DALBase
    {
        public Guid Save(PayingEntity entity)
        {
            try
            {
                entity.CreatedOn = entity.UpdatedOn = GeneralHelper.CurrentDate();
                _dbContext.PayingEntities.Add(entity);
                _dbContext.SaveChanges();
            }
            catch(Exception ex)
            {
                // Save log
            }
            return entity.Id;
        }



        public Guid Update(PayingEntity entity)
        {
            Guid returnId = Guid.Empty;
            var existingEntity = List().Where(w => w.Id.Equals(entity.Id)).FirstOrDefault();
            if (existingEntity != null)
            {
                existingEntity.Description = entity.Description;
                existingEntity.Abbreviation = entity.Abbreviation;
                existingEntity.LogoName = string.IsNullOrEmpty(entity.LogoName) ? existingEntity.LogoName : entity.LogoName;
                existingEntity.UpdatedOn = GeneralHelper.CurrentDate();
                _dbContext.SaveChanges();
                returnId = existingEntity.Id;
                //PayingEntity newEntity = new PayingEntity(entity.Name, entity.Abbreviation, true)
                //{
                //    Description = entity.Description,
                //    LogoPath = string.IsNullOrEmpty(entity.LogoPath) ? existingEntity.LogoPath : entity.LogoPath,
                //    PreviousDetails = existingEntity,
                //    IsEnabled = existingEntity.IsEnabled
                //};
                //existingEntity.IsEnabled = false;
                //Save(newEntity);
                //returnId = newEntity.Id;
            }
            return returnId;
        }


        public IQueryable<PayingEntity> List()
        {
            return _dbContext.PayingEntities;
        }

        public IQueryable<PayingEntity> ListActive()
        {
            return _dbContext.PayingEntities.Where(w => w.IsEnabled);
        }

        public IQueryable<EntityAmountRange> ListEntityAmountRanges()
        {
            return _dbContext.EntityAmountRanges;
        }
        

        public PayingEntity FindById(Guid id)
        {
            PayingEntity obj = null;
            if (!id.Equals(Guid.Empty))
            {
                obj = _dbContext.PayingEntities.Where(w => w.Id.Equals(id)).FirstOrDefault();
            }
            return obj;
        }

        public bool ChangeStatus(Guid id, bool status)
        {
            bool success = false;
            if (!id.Equals(Guid.Empty))
            {
                PayingEntity obj = _dbContext.PayingEntities.Where(w => w.Id.Equals(id)).FirstOrDefault();
                if (obj != null)
                {
                    obj.IsEnabled = status;
                    _dbContext.SaveChanges();
                    success = true;
                }
            };
            return success;
        }

        internal bool AddRange(EntityAmountRange range)
        {
            var success = false;
            var PayingEntity = List().Where(w => w.Id.Equals(range.PayingEntityId)).FirstOrDefault();
            if (PayingEntity != null)
            {
                PayingEntity.RangeConfig.Add(range);
                _dbContext.SaveChanges();
                success = true;
            }
            return success;
        }

        internal bool AddRangeEmail(Guid rangeId, List<string> emails)
        {
            var success = false;
            var PayingEntity = List().Where(w => w.RangeConfig.Any(a=>a.Id.Equals(rangeId))).FirstOrDefault();
            if (PayingEntity != null)
            {
                var rangeConfig = PayingEntity.RangeConfig.Where(w => w.Id.Equals(rangeId)).FirstOrDefault();
                rangeConfig.EmailAddresses.AddRange(emails.Select(s=>new AmountRangeEmail {
                    EntityAmountRangeId = rangeConfig.Id,
                    Email = s
                }).ToList());
                _dbContext.SaveChanges();
                success = true;
            }
            return success;
        }

        internal void DeleteEmailFromRange(Guid rangeId, string email)
        {
            var rangeConfig = ListEntityAmountRanges().Where(w => w.Id.Equals(rangeId)).FirstOrDefault();
            if (rangeConfig != null)
            {
                var emailObj = rangeConfig.EmailAddresses.Where(w => !w.IsDeleted && w.Email.Equals(email, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (emailObj != null)
                {
                    emailObj.IsDeleted = true;
                    _dbContext.SaveChanges();
                }
            }
        }
    }
}
