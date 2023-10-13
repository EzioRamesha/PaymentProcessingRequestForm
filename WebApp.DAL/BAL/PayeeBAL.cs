using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.DAL;
using WebApp.DAL.Helpers;
using WebApp.DAL.Models;
using WebApp.DAL.Models.ResponseModels;

namespace WebApp.DAL.BAL
{
    public class PayeeBAL
    {
        private static readonly PayeeDAL _payeeDAL = new PayeeDAL();

        public ManagePayeeViewModel GetAll(ManagePayeeViewModel model)
        {
            try
            {
                if(model.Pager==null)
                {
                    model.Pager.CurrentPage = 1;
                    model.Pager.TotalItems = 1;
                    model.Pager.ItemsPerPage = 20;
                }
                var list = _payeeDAL.List().Where(w => w.PreviousEdits.Count == 0);
                model.Pager.TotalItems = list.Count();
                model.Payees.AddRange(list.OrderBy(o => o.Name)
                                            .Skip((model.Pager.CurrentPage - 1) * model.Pager.ItemsPerPage)
                                            .Take(model.Pager.ItemsPerPage)
                                            .Select(s => new PayeeModel
                                            {
                                                Id = s.Id.ToString(),
                                                Name = s.Name,
                                                Phone = s.Phone,
                                                Fax = s.Fax,
                                                AddressLine1 = s.AddressLine1,
                                                AddressLine2 = s.AddressLine2,
                                                AddressLine3 = s.AddressLine3,
                                                IsEnabled = s.IsEnabled,
                                                HotelName = s.HotelName,
                                                CountryName = s.Country == null ? string.Empty : s.Country.Name,
                                                CountryId = s.CountryId
                                            }).ToList());
                return model;
            }
            catch (Exception)
            {
                return model;
            }
           finally
            {

            }
         
        }

        public ManagePayeeViewModel GetAllFilterByName(ManagePayeeViewModel model,string PayeeName)
        {
            try
            {
                if (model.Pager == null)
                {
                    model.Pager.CurrentPage = 1;
                    model.Pager.TotalItems = 1;
                    model.Pager.ItemsPerPage = 20;
                }
                var list = _payeeDAL.List().Where(w => w.Id.ToString().Equals(PayeeName.ToString()));
                model.Pager.TotalItems = list.Count();
                model.Payees.AddRange(list.OrderBy(o => o.Name)
                                            .Skip((model.Pager.CurrentPage - 1) * model.Pager.ItemsPerPage)
                                            .Take(model.Pager.ItemsPerPage)
                                            .Select(s => new PayeeModel
                                            {
                                                Id = s.Id.ToString(),
                                                Name = s.Name,
                                                Phone = s.Phone,
                                                Fax = s.Fax,
                                                AddressLine1 = s.AddressLine1,
                                                AddressLine2 = s.AddressLine2,
                                                AddressLine3 = s.AddressLine3,
                                                IsEnabled = s.IsEnabled,
                                                HotelName = s.HotelName,
                                                CountryName = s.Country == null ? string.Empty : s.Country.Name,
                                                CountryId = s.CountryId
                                            }).ToList());
                return model;
            }
            catch (Exception)
            {
                return null;
            }
            finally {
               
            }
           
            
        }


        public List<PayeeModel> GetActive()
        {
            try
            {
                return _payeeDAL.List().Where(s => s.IsEnabled).Select(s => new PayeeModel
                {
                    Id = s.Id.ToString(),
                    Name = s.Name,
                    Phone = s.Phone,
                    Fax = s.Fax,
                    AddressLine1 = s.AddressLine1,
                    AddressLine2 = s.AddressLine2,
                    AddressLine3 = s.AddressLine3,
                    IsEnabled = s.IsEnabled,
                    HotelName = s.HotelName,
                    CountryName = s.Country == null ? string.Empty : s.Country.Name,
                    CountryId = s.CountryId
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {

            }
  
        }

        public PayeeModel GetPayeeInfo(string PayeeBankAccountDetailId)
        {
            PayeeModel _payee = new PayeeModel();

            try
            {
                var payeeAccID = Guid.Parse(PayeeBankAccountDetailId);
                Guid PayeeID = _payeeDAL.PayeeBankAccountList().Where(s => s.Id.Equals(payeeAccID)).FirstOrDefault().PayeeId;

                _payee = PayeeModel.ToModel(_payeeDAL.List().Where(s => s.Id.Equals(PayeeID)).FirstOrDefault());
            }
            catch (Exception e)
            {
                string str = e.Message;
            }

            return _payee;
        }

        public ResponseObject<CreatePayeeResponseModel> CreatePayee(NewPayeeViewModel model)
        {
            ResponseObject<CreatePayeeResponseModel> response;
            bool IsSuccess = false;
            try
            {
                Guid Id = _payeeDAL.Save(model);
                IsSuccess = !Id.Equals(Guid.Empty);
            }
            catch (Exception)
            {
                IsSuccess = false;
            }
            if (IsSuccess)
            {
                response = new ResponseObject<CreatePayeeResponseModel>
                {
                    Message = "Payee created successfully.",
                    ResponseType = "success"
                };
            }
            else
            {
                response = new ResponseObject<CreatePayeeResponseModel>
                {
                    Message = "Something went wrong while creating the payee.",
                    ResponseType = "error"
                };
            }
            return response;
        }

        public ResponseObject<UpdatePayeeResponseModel> UpdatePayee(UpdatePayeeViewModel model)
        {
            ResponseObject<UpdatePayeeResponseModel> response;
            bool IsSuccess = false;
            try
            {
                var Id = _payeeDAL.Update(model);
                IsSuccess = !Id.Equals(Guid.Empty);
            }
            catch (Exception)
            {
                IsSuccess = false;
            }
            if (IsSuccess)
            {
                response = new ResponseObject<UpdatePayeeResponseModel>
                {
                    Message = "Payee updated successfully.",
                    ResponseType = "success"
                };
            }
            else
            {
                response = new ResponseObject<UpdatePayeeResponseModel>
                {
                    Message = "Something went wrong while updating the payee.",
                    ResponseType = "error"
                };
            }
            return response;
        }

        public bool Disable(PayeeModel payingEntity)
        {
            var isDisabled = false;
            if (payingEntity != null)
            {
                Guid id = payingEntity.Id.ToGuid();
                isDisabled = _payeeDAL.ChangeStatus(id, false);
            }
            return isDisabled;
        }

        public bool Enable(PayeeModel payee)
        {
            var isEnabled = false;
            if (payee != null)
            {
                Guid id = payee.Id.ToGuid();
                isEnabled = _payeeDAL.ChangeStatus(id, true);
            }
            return isEnabled;
        }

        public List<PayeeBankDetails> GetAccountDetails(PayeeModel payee)
        {
            var bankAccounts = new List<PayeeBankDetails>();
            if (payee != null)
            {
                Guid id = payee.Id.ToGuid();
                using (var _payeeDAL = new PayeeDAL())
                {
                    var x = _payeeDAL.List().Where(w => w.Id.Equals(id)).FirstOrDefault();
                    if (x != null)
                    {
                        if (x.PayeeBankAccounts != null)
                        {
                            bankAccounts.AddRange(x.PayeeBankAccounts.Where(w => w.IsEnabled).Select(s => new PayeeBankDetails
                            {
                                Id = s.Id.ToString(),
                                AccountName = s.AccountName,
                                AccountNumber = s.AccountNumber,
                                BankName = s.BankName,
                                IBAN = s.IBAN,
                                IFSC = s.IFSC,
                                Swift = s.Swift,
                                AccountType = s.AccountType,
                                BankAddress = s.BankAddress
                            }));
                        }
                    }
                    //while (x != null)
                    //{
                    //    var accounts = x.PayeeBankAccounts == null ? null : x.PayeeBankAccounts.Where(w => w.IsEnabled).Select(s => new PayeeBankDetails
                    //    {
                    //        Id = s.Id.ToString(),
                    //        AccountName = s.AccountName,
                    //        AccountNumber = s.AccountNumber,
                    //        BankName = s.BankName,
                    //        IBAN = s.IBAN,
                    //        IFSC = s.IFSC,
                    //        Swift = s.Swift,
                    //        AccountType = s.AccountType
                    //    }).ToList();
                    //    bankAccounts.AddRange(accounts);
                    //    id = x.PreviousDetailsId.HasValue ? x.PreviousDetailsId.Value : Guid.Empty;
                    //    x = _payeeDAL.List().Where(w => w.Id.Equals(id)).FirstOrDefault();
                    //}
                }

            }
            return bankAccounts;
        }

        public ResponseObject<UpdatePayeeBankDetailsResponseModel> UpdatePayeeAccountDetails(PayeeDetailsViewModel model)
        {
            ResponseObject<UpdatePayeeBankDetailsResponseModel> response = null;
            var returnValue = false;
            if (model != null)
            {
                var payee = new Data.Payee
                {
                    Id = model.Id.ToGuid()
                };
                if (payee.Id != null && payee.Id != Guid.Empty)
                {
                    payee.PayeeBankAccounts = new List<Data.PayeeBankAccountDetail>();

                    if (model.AccountDetails != null)
                        payee.PayeeBankAccounts.AddRange(model.AccountDetails.Select(s => new Data.PayeeBankAccountDetail
                        {
                            AccountName = s.AccountName,
                            AccountNumber = s.AccountNumber,
                            AccountType = s.AccountType,
                            BankAddress = s.BankAddress,
                            BankName = s.BankName,
                            IBAN = s.IBAN,
                            Id = s.Id == null ? Guid.Empty : s.Id.ToGuid(),
                            IFSC = s.IFSC,
                            Swift = s.Swift
                        }).ToList());
                    returnValue = _payeeDAL.UpdateBankAccountDetails(payee);
                    if (returnValue)
                        response = new ResponseObject<UpdatePayeeBankDetailsResponseModel>
                        {
                            ResponseType = "success",
                            Message = "Payee bank details updated"
                        };
                    else
                        response = new ResponseObject<UpdatePayeeBankDetailsResponseModel>
                        {
                            ResponseType = "error",
                            Message = "Something went wrong while updating the bank details!"
                        };
                }
            }
            return response;
        }



        public ResponseObject<AddPayeeBankResponse> AddOrUpdatePayeeBank(PayeeBankViewModel model)
        {
            ResponseObject<AddPayeeBankResponse> response = new ResponseObject<AddPayeeBankResponse>();
            Guid bankDetailsId = Guid.Empty;
            if (model != null)
            {
                using (var _payeeDAL = new PayeeDAL())
                {
                    try
                    {
                        bankDetailsId = _payeeDAL.AddOrUpdatePayeeBank(model);
                    }
                    catch (Exception ex)
                    {
                        response = new ResponseObject<AddPayeeBankResponse>
                        {
                            ResponseType = "error",
                            Message = "Something went wrong while adding the payee bank details"
                        };
                    }

                }
                if (!bankDetailsId.Equals(Guid.Empty))
                {
                    response = new ResponseObject<AddPayeeBankResponse>
                    {
                        ResponseType = "success",
                        Message = "Payee bank details added successfully"
                    };
                }
                else
                {
                    response = new ResponseObject<AddPayeeBankResponse>
                    {
                        ResponseType = "error",
                        Message = "Unable to add the bank details"
                    };
                }
            }
            else
            {
                response = new ResponseObject<AddPayeeBankResponse>
                {
                    ResponseType = "error",
                    Message = "Invalid data provided"
                };
            }
            return response;
        }


        public ResponseObject<RemovePayeeBankResposne> RemovePayeeBank(PayeeBankViewModel model)
        {
            ResponseObject<RemovePayeeBankResposne> response = null;
            Guid bankDetailsId = Guid.Empty;
            if (model != null)
            {
                using (var _payeeDAL = new PayeeDAL())
                {
                    try
                    {
                        bankDetailsId = _payeeDAL.RemovePayeeBank(model);
                    }
                    catch (Exception ex)
                    {
                        response = new ResponseObject<RemovePayeeBankResposne>
                        {
                            ResponseType = "error",
                            Message = "Something went wrong while removing the payee bank details"
                        };
                    }
                    if (!bankDetailsId.Equals(Guid.Empty))
                    {
                        response = new ResponseObject<RemovePayeeBankResposne>
                        {
                            ResponseType = "success",
                            Message = "Payee bank details removed successfully"
                        };
                    }
                    else
                    {
                        response = new ResponseObject<RemovePayeeBankResposne>
                        {
                            ResponseType = "error",
                            Message = "Unable to remove the bank details"
                        };
                    }
                }
            }
            else
            {
                response = new ResponseObject<RemovePayeeBankResposne>
                {
                    ResponseType = "error",
                    Message = "Invalid data provided"
                };
            }
            return response;
        }
    }
}
