using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.DAL;
using WebApp.DAL.Models;
using WebApp.DAL.Helpers;
using System.Configuration;
using WebApp.DAL.Models.ResponseModels;

namespace WebApp.DAL.BAL
{
    public class PayingEntityBAL
    {
        //private readonly PayingEntityDAL _payingEntityDAL = new PayingEntityDAL();
        public ResponseObject<CreatePayingEntityResponseModel> Save(AddPayingEntityViewModel model)
        {
            ResponseObject<CreatePayingEntityResponseModel> response;
            Guid returnId;
            var payingEntity = new Data.PayingEntity(model.Name)
            {
                Abbreviation = model.Abbreviation,
                Description = model.Description
            };
            using (var _payingEntityDAL = new PayingEntityDAL())
            {
                returnId = _payingEntityDAL.Save(payingEntity);
            }
            //var returnId = _payingEntityDAL.Save(payingEntity);
            if (!returnId.IsNullOrEmpty())
            {
                if (model.Logo != null)
                {
                    var path = getLogoPathLocal(model.Logo.FileName, payingEntity.Id.ToString());
                    model.Logo.SaveAs(path);
                    payingEntity.LogoName = payingEntity.Id.ToString() + Path.GetExtension(model.Logo.FileName);
                    using (var _payingEntityDAL = new PayingEntityDAL())
                    {
                        _payingEntityDAL.Update(payingEntity);
                    }
                }
                response = new ResponseObject<CreatePayingEntityResponseModel>
                {
                    ResponseType = "success",
                    Message = "Successfully created the Paying Entity"
                };
            }
            else
            {
                response = new ResponseObject<CreatePayingEntityResponseModel>
                {
                    ResponseType = "error",
                    Message = "Something went wrong while creating the paying entity"
                };
            }
            return response;
        }


        public ResponseObject<UpdatePayingEntityResponseModel> Update(UpdatePayingEntityViewModel model)
        {
            ResponseObject<UpdatePayingEntityResponseModel> response;
            var payingEntity = new Data.PayingEntity(model.Name, model.Abbreviation)
            {
                Id = model.Id.ToGuid(),
                Description = model.Description
            };
            if (model.Logo != null)
            {
                try
                {
                    var path = getLogoPathLocal(model.Logo.FileName, payingEntity.Id.ToString());
                    model.Logo.SaveAs(path);
                    payingEntity.LogoName = payingEntity.Id.ToString() + Path.GetExtension(model.Logo.FileName);
                }
                catch (Exception e)
                {
                    response = new ResponseObject<UpdatePayingEntityResponseModel>
                    {
                        ResponseType = "error",
                        Message = "Some error occurred while trying to save the image. Edit cancelled.\n" + e.Message
                    };
                    //File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory+"/logs/log-" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt", e.Message);
                    return response;
                }
            }
            Guid id;
            using (var _payingEntityDAL = new PayingEntityDAL())
            {
                id = _payingEntityDAL.Update(payingEntity);
            }
            if (!id.Equals(Guid.Empty))
            {
                response = new ResponseObject<UpdatePayingEntityResponseModel>
                {
                    ResponseType = "success",
                    Message = "Successfully updated the paying entity"
                };
            }
            else
            {
                response = new ResponseObject<UpdatePayingEntityResponseModel>
                {
                    ResponseType = "error",
                    Message = "Something went wrong while updating the paying entity"
                };
            }
            return response;
        }


        public List<PayingEntity> GetActive()
        {
            List<PayingEntity> response = new List<PayingEntity>();
            using (var _payingEntityDAL = new PayingEntityDAL())
            {
                response = _payingEntityDAL.ListActive().Where(w => w.PreviousEdits.Count == 0).Select(s => new PayingEntity
                {
                    Id = s.Id.ToString(),
                    Name = s.Name,
                    Description = s.Description,
                    Abbreviation = s.Abbreviation,
                    IsEnabled = s.IsEnabled
                }).ToList();
            }
            return response;
        }
        public ManagePayingEntitiesViewModel GetAll(ManagePayingEntitiesViewModel model)
        {
            using (var _payingEntityDAL = new PayingEntityDAL())
            {
                var list = _payingEntityDAL.List().Where(w => w.PreviousEdits.Count == 0);
                model.Pager.TotalItems = list.Count();
                model.PayingEntities.AddRange(list.OrderBy(w => w.Name)
                                                  .Skip((model.Pager.CurrentPage - 1) * model.Pager.ItemsPerPage)
                                                  .Take(model.Pager.ItemsPerPage)
                                                  .Select(s => new PayingEntity
                                                  {
                                                      Id = s.Id.ToString(),
                                                      Name = s.Name,
                                                      Description = s.Description,
                                                      Abbreviation = s.Abbreviation,
                                                      IsEnabled = s.IsEnabled
                                                  }).ToList());
            }
            //var emptyGuid = Guid.Empty;
            return model;
        }

        public bool Disable(PayingEntity payingEntity)
        {
            var isDisabled = false;
            if (payingEntity != null)
            {
                Guid id = payingEntity.Id.ToGuid();
                using (var _payingEntityDAL = new PayingEntityDAL())
                {
                    isDisabled = _payingEntityDAL.ChangeStatus(id, false);
                }
            }
            return isDisabled;
        }

        public bool Enable(PayingEntity payingEntity)
        {
            var isEnabled = false;
            if (payingEntity != null)
            {
                Guid id = payingEntity.Id.ToGuid();
                using (var _payingEntityDAL = new PayingEntityDAL())
                {
                    isEnabled = _payingEntityDAL.ChangeStatus(id, true);
                }
            }
            return isEnabled;
        }


        private string getLogoPathLocal(string logoFile, string newName)
        {
            var extension = Path.GetExtension(logoFile);
            //var path = Path.Combine(ConfigurationManager.AppSettings["baseURL"], ConfigurationManager.AppSettings["LogosPath"].ToString(), newName + extension);
            //var path = Path.Combine(ConfigurationManager.AppSettings["LogosPath"].ToString(), newName + extension);
            var path = Path.Combine(Constants.PAYING_ENTITY_LOGO_BASE_PATH, newName + extension);
            return path;
        }

        public ViewEntityAmountRange GetRangeConfigurationFor(string payingEntityId)
        {
            ViewEntityAmountRange response = null;
            try
            {
                Guid ID = Guid.Parse(payingEntityId);
                using (var _payingEntityDAL = new PayingEntityDAL())
                {
                    var entity = _payingEntityDAL.List().Where(w => w.Id.Equals(ID)).FirstOrDefault();
                    response = new ViewEntityAmountRange
                    {
                        PayingEntity = new PayingEntity
                        {
                            Name = entity.Name,
                            Abbreviation = entity.Abbreviation,
                            Description = entity.Description,
                            Id = entity.Id.ToString(),
                            IsEnabled = entity.IsEnabled,
                        },
                        AmountRangeConfig = entity.RangeConfig.Select(s => new RangeEmails
                        {
                            Id = s.Id.ToString(),
                            AmountFrom = s.AmountRangeFrom,
                            AmountTo = s.AmountRangeTo,
                            EmailAddresses = s.EmailAddresses.Where(w => !w.IsDeleted).Select(s1 => s1.Email).ToList()
                        }).ToList()
                    };
                }
            }
            catch (Exception e)
            {
                response = null;
            }
            return response;
        }

        public ResponseObject<AddRange> AddEntityAmountRange(AddRange range)
        {
            ResponseObject<AddRange> response = null;
            try
            {
                var id = Guid.Parse(range.PayingEntityId);
                ViewEntityAmountRange entity = null;
                using (var _payingEntityDAL = new PayingEntityDAL())
                {
                    entity = _payingEntityDAL.List().Where(w => w.Id.Equals(id)).Select(s => new ViewEntityAmountRange
                    {
                        PayingEntity = new PayingEntity
                        {
                            Id = s.Id.ToString(),
                            Abbreviation = s.Abbreviation,
                            Description = s.Description,
                            Name = s.Name
                        },
                        AmountRangeConfig = s.RangeConfig.Select(s1 => new RangeEmails
                        {
                            AmountFrom = s1.AmountRangeFrom,
                            AmountTo = s1.AmountRangeTo,
                            EmailAddresses = s1.EmailAddresses.Where(w => !w.IsDeleted).Select(s2 => s2.Email).ToList()
                        }).ToList()
                    }).FirstOrDefault();
                }
                if (entity != null)
                {
                    if (entity.AmountRangeConfig.Any(a => a.AmountFrom == range.AmountFrom && a.AmountTo == range.AmountTo))
                    {
                        response = new ResponseObject<AddRange>
                        {
                            Message = "Range already exists",
                            ResponseType = "error"
                        };
                    }
                    else
                    {
                        var success = false;
                        using (var _payingEntityDAL = new PayingEntityDAL())
                        {
                            success = _payingEntityDAL.AddRange(new Data.EntityAmountRange
                            {
                                AmountRangeFrom = range.AmountFrom,
                                AmountRangeTo = range.AmountTo,
                                PayingEntityId = Guid.Parse(range.PayingEntityId)
                            });
                        }

                        if (success)
                            response = new ResponseObject<AddRange>
                            {
                                Message = "Range added",
                                ResponseType = "success"
                            };
                        else throw new Exception();
                    }
                }
            }
            catch (Exception e)
            {
                response = new ResponseObject<AddRange>
                {
                    ResponseType = "error",
                    Message = "Something went wrong while processing your request"
                };
            }
            return response;
        }

        public ResponseObject<AddEmail> AddEmailForRange(AddEmail rangeEmail)
        {
            ResponseObject<AddEmail> response = null;
            try
            {
                var emailExists = false;
                var rangeId = Guid.Parse(rangeEmail.rangeId);
                using (var _payingEntityDAL = new PayingEntityDAL())
                {
                    emailExists = _payingEntityDAL.ListEntityAmountRanges()
                                                    .Any(w => w.Id.Equals(rangeId) && w.EmailAddresses.Where(w1 => !w1.IsDeleted).Any(a => rangeEmail.Emails.Contains(a.Email)));
                }
                if (!emailExists)
                {
                    using (var _payingEntityDAL = new PayingEntityDAL())
                    {
                        _payingEntityDAL.AddRangeEmail(rangeId, rangeEmail.Emails.ToList());
                    }
                    response = new ResponseObject<AddEmail>
                    {
                        Message = "Email(s) added to the range",
                        ResponseType = "success"
                    };
                }
                else
                {
                    response = new ResponseObject<AddEmail>
                    {
                        Message = "The email already exists for this range",
                        ResponseType = "error"
                    };
                }
            }
            catch (Exception e)
            {
                response = new ResponseObject<AddEmail>
                {
                    ResponseType = "error",
                    Message = "Something went wrong while processing your request"
                };
            }
            return response;
        }

        public ResponseObject<DeleteEmailFromRange> DeleteEmailFromRange(DeleteEmailFromRange model)
        {
            ResponseObject<DeleteEmailFromRange> response = null;
            try
            {
                bool? emailExists = false;
                var rangeId = Guid.Parse(model.RangeId);
                using (var _payingEntityDAL = new PayingEntityDAL())
                {
                    emailExists = _payingEntityDAL.ListEntityAmountRanges()
                                            .Where(w => w.Id.Equals(rangeId))
                                                            .FirstOrDefault()
                                                            ?.EmailAddresses
                                                            ?.Any(a => !a.IsDeleted && a.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase));
                }
                if (emailExists.HasValue && emailExists.Value)
                {
                    using (var _payingEntityDAL = new PayingEntityDAL())
                    {
                        _payingEntityDAL.DeleteEmailFromRange(rangeId, model.Email);
                    }
                    response = new ResponseObject<Models.DeleteEmailFromRange>
                    {
                        ResponseType = "success",
                        Message = "Successfully deleted the email"
                    };
                }
                else
                {
                    response = new ResponseObject<Models.DeleteEmailFromRange>
                    {
                        ResponseType = "error",
                        Message = "Email address not found in the range"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new ResponseObject<Models.DeleteEmailFromRange>
                {
                    ResponseType = "error",
                    Message = "Something went wrong while processing your request"
                };
            }
            return response;
        }

        public ResponseObject<RangeEmails> GetRangeDetailsForRangeId(string rangeId)
        {
            ResponseObject<RangeEmails> response = null;
            try
            {
                var RangeId = Guid.Parse(rangeId);
                using (var _payingEntityDAL = new PayingEntityDAL())
                {
                    var range = _payingEntityDAL.ListEntityAmountRanges().Where(w => w.Id.Equals(RangeId)).FirstOrDefault();
                    if (range != null)
                    {
                        response = new ResponseObject<RangeEmails>
                        {
                            Data = new RangeEmails
                            {
                                AmountFrom = range.AmountRangeFrom,
                                AmountTo = range.AmountRangeTo,
                                EmailAddresses = range.EmailAddresses.Where(w => !w.IsDeleted).Select(s => s.Email).ToList(),
                                Id = range.Id.ToString()
                            },
                            Message = "Range located",
                            ResponseType = "success"
                        };
                    }
                }
                if (response == null)
                {
                    response = new ResponseObject<RangeEmails>
                    {
                        Message = "Range could not be located",
                        ResponseType = "error"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new ResponseObject<RangeEmails>
                {
                    ResponseType = "error",
                    Message = "Something went wrong while processing your request"
                };
            }
            return response;
        }

        internal List<string> GetEmailsForEntityWithRange(string payingEntityCode, decimal totalUSD)
        {
            List<string> emails = new List<string>(); 
            using (var _payingEntityDAL = new PayingEntityDAL())
            {
                var payingEntity = _payingEntityDAL.List().Where(w => w.Abbreviation.Equals(payingEntityCode, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (payingEntity != null)
                {
                    var range = payingEntity.RangeConfig.Where(w => w.AmountRangeFrom <= totalUSD && w.AmountRangeTo >= totalUSD).ToList();
                    if (range != null)
                    {
                        range.ForEach(r =>
                        {
                            emails.AddRange(r.EmailAddresses.Where(w=>!w.IsDeleted).Select(s => s.Email));
                        });
                    }
                }
                //emails.AddRange(_payingEntityDAL.ListEntityAmountRanges()
                //    .Where(w => w.PayingEntity.Abbreviation.Equals(payingEntityCode, StringComparison.OrdinalIgnoreCase))
                //    .Where(w => w.AmountRangeFrom < totalUSD && w.AmountRangeTo > totalUSD)
                //    .FirstOrDefault()?.EmailAddresses.Select(s=>s.Email).ToList());
            }
            return emails;
        }
    }
}
