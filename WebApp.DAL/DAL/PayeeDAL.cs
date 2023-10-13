using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;
using WebApp.DAL.Helpers;
using WebApp.DAL.Models;

namespace WebApp.DAL.DAL
{
    internal class PayeeDAL : DALBase
    {
        public IQueryable<Data.Payee> List()
        {
            return _dbContext.Payees;
        }

        public IQueryable<Data.PayeeBankAccountDetail> PayeeBankAccountList()
        {
            return _dbContext.PayeeBankAccountDetails;
        }

        internal Guid Save(NewPayeeViewModel model)
        {
            var returnVal = Guid.Empty;
            try
            {
                var payee = new Data.Payee
                {
                    Name = model.Name,
                    Phone = model.Phone,
                    AddressLine1 = model.AddressLine1,
                    AddressLine2 = model.AddressLine2,
                    AddressLine3 = model.AddressLine3,
                    Fax = model.Fax,
                    CountryId = string.IsNullOrEmpty(model.CountryId) ? null : (Guid?)model.CountryId.ToGuid(),
                    HotelName = model.HotelName
                };
                _dbContext.Payees.Add(payee);
                _dbContext.SaveChanges();
                returnVal = payee.Id;
            }
            catch (Exception ex)
            {
                returnVal = Guid.Empty;
            }
            return returnVal;
        }

        internal Guid Update(UpdatePayeeViewModel payee)
        {
            var returnVal = Guid.Empty;
            //bool IsSuccess = false;
            try
            {
                var idToSearch = payee.Id.ToGuid();
                var existingPayee = _dbContext.Payees.Where(w => w.Id.Equals(idToSearch)).FirstOrDefault();
                if (existingPayee != null)
                {
                    existingPayee.Name = payee.Name;
                    existingPayee.Phone = payee.Phone;
                    existingPayee.AddressLine1 = payee.AddressLine1;
                    existingPayee.AddressLine2 = payee.AddressLine2;
                    existingPayee.AddressLine3 = payee.AddressLine3;
                    existingPayee.Fax = payee.Fax;
                    existingPayee.CountryId = string.IsNullOrEmpty(payee.CountryId) ? null : (Guid?)payee.CountryId.ToGuid();
                    existingPayee.HotelName = payee.HotelName;
                    _dbContext.SaveChanges();
                    returnVal = existingPayee.Id;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return returnVal;
        }


        internal bool ChangeStatus(Guid id, bool status)
        {
            bool success = false;
            if (!id.Equals(Guid.Empty))
            {
                Data.Payee obj = _dbContext.Payees.Where(w => w.Id.Equals(id)).FirstOrDefault();
                if (obj != null)
                {
                    obj.IsEnabled = status;
                    _dbContext.SaveChanges();
                    success = true;
                }
            };
            return success;
        }

        internal List<PayeeBankAccountDetail> GetPayeeAccounts(Guid id)
        {
            var bankAccounts = new List<PayeeBankAccountDetail>();
            if (!id.Equals(Guid.Empty))
            {
                var payee = _dbContext.Payees.Where(w => w.Id.Equals(id)).FirstOrDefault();
                if (payee != null)
                {
                    while (!payee.PreviousDetailsId.Equals(null))
                    {
                        id = (Guid)payee.PreviousDetailsId;
                        payee = _dbContext.Payees.Where(w => w.Id.Equals(id)).FirstOrDefault();
                        bankAccounts.AddRange(payee.PayeeBankAccounts.Where(w => w.IsEnabled));
                        // TODO: get bank accounts
                    }
                }
            }
            return bankAccounts;
        }

        internal bool UpdateBankAccountDetails(Data.Payee payee)
        {
            var returnValue = false;
            if (payee != null)
            {
                var id = payee.Id;
                var existingPayee = List().Where(w => w.Id.Equals(id)).FirstOrDefault();

                if (existingPayee != null)
                {
                    var ids = payee.PayeeBankAccounts.Where(w => !w.Id.Equals(Guid.Empty)).Select(s => s.Id);

                    existingPayee.PayeeBankAccounts.ToList().ForEach(f => f.IsEnabled = false);
                    payee.PayeeBankAccounts.ForEach(f =>
                    {
                        f.PreviousDetails = existingPayee.PayeeBankAccounts.Where(w => w.Id.Equals(f.Id)).FirstOrDefault();
                    });
                    existingPayee.PayeeBankAccounts.AddRange(payee.PayeeBankAccounts);
                    _dbContext.SaveChanges();
                    returnValue = true;
                }
            }
            return returnValue;
        }

        internal bool UpdateBankAccountDetails(Models.PayeeDetailsViewModel model)
        {
            var returnValue = false;
            if (model != null)
            {
                var id = model.Id.ToGuid();
                var existingPayee = List().Where(w => w.Id.Equals(id)).FirstOrDefault();
                if (existingPayee != null)
                {
                    var updatedIds = model.AccountDetails.Select(s => s.Id.ToGuid()).ToList();
                    var existingIds = existingPayee.PayeeBankAccounts.Select(s => s.Id).ToList().Select(s => s.ToString()).ToList();


                    // disable removed accounts
                    var accountsToDelete = existingPayee.PayeeBankAccounts.Where(w => !updatedIds.Contains(w.Id)).ToList();
                    accountsToDelete.ForEach(f =>
                    {
                        f.IsEnabled = false;
                    });

                    // add new accounts
                    var accountsToAdd = model.AccountDetails.Where(w => !existingIds.Contains(w.Id)).ToList();
                    existingPayee.PayeeBankAccounts.AddRange(accountsToAdd.Select(s => new PayeeBankAccountDetail
                    {
                        AccountName = s.AccountName,
                        AccountNumber = s.AccountNumber,
                        AccountType = s.AccountType,
                        BankAddress = s.BankAddress,
                        BankName = s.BankName,
                        IBAN = s.IBAN,
                        IFSC = s.IFSC,
                        Swift = s.Swift
                    }).ToList());


                    //var accountsToEdit = model.AccountDetails.Where(w => existingIds.Contains(w.Id)).ToList();

                    // edit existing accounts
                    existingPayee.PayeeBankAccounts.Where(w => updatedIds.Contains(w.Id)).ToList().ForEach(f =>
                    {
                        var newAccount = model.AccountDetails.Where(w => w.Id == f.Id.ToString()).FirstOrDefault();
                        f.AccountName = newAccount.AccountName;
                        f.AccountNumber = newAccount.AccountNumber;
                        f.AccountType = newAccount.AccountType;
                        f.BankAddress = newAccount.BankAddress;
                        f.BankName = newAccount.BankName;
                        f.IBAN = newAccount.IBAN;
                        f.IFSC = newAccount.IFSC;
                        f.Swift = newAccount.Swift;
                    });
                    _dbContext.SaveChanges();
                    returnValue = true;
                }
            }
            return returnValue;
        }

        internal Guid AddOrUpdatePayeeBank(Models.PayeeBankViewModel model)
        {
            var guid = Guid.Empty;
            if (model != null)
            {
                try
                {
                    var payeeId = model.Id.ToGuid();
                    var existingPayee = _dbContext.Payees.Where(w => w.Id.Equals(payeeId)).FirstOrDefault();
                    if (existingPayee != null)
                    {
                        var bankId = model.AccountDetails.Id.ToGuid();
                        //var existingBank = existingPayee.PayeeBankAccounts.Where(w => w.Id.Equals(bankId)).FirstOrDefault();
                        var bankDetails = new PayeeBankAccountDetail
                        {
                            PayeeId = payeeId,
                            AccountName = model.AccountDetails.AccountName,
                            AccountNumber = model.AccountDetails.AccountNumber,
                            AccountType = model.AccountDetails.AccountType,
                            BankAddress = model.AccountDetails.BankAddress,
                            BankName = model.AccountDetails.BankName,
                            IBAN = model.AccountDetails.IBAN,
                            IFSC = model.AccountDetails.IFSC,
                            Swift = model.AccountDetails.Swift
                        };
                        if (bankId != null && !bankId.Equals(Guid.Empty))
                        {
                            bankDetails.Id = bankId;//existingBank.Id;
                            _dbContext.PayeeBankAccountDetails.Attach(bankDetails);
                            var manager = ((IObjectContextAdapter)_dbContext).ObjectContext.ObjectStateManager;
                            manager.ChangeObjectState(bankDetails, EntityState.Modified);
                            //existingBank.AccountName = model.AccountDetails.AccountName;
                            //existingBank.AccountNumber = model.AccountDetails.AccountNumber;
                            //existingBank.AccountType = model.AccountDetails.AccountType;
                            //existingBank.BankAddress = model.AccountDetails.BankAddress;
                            //existingBank.BankName = model.AccountDetails.BankName;
                            //existingBank.IBAN = model.AccountDetails.IBAN;
                            //existingBank.IFSC = model.AccountDetails.IFSC;
                            //existingBank.Swift = model.AccountDetails.Swift;
                        }
                        else
                        {
                            _dbContext.PayeeBankAccountDetails.Add(bankDetails);
                            //existingPayee.PayeeBankAccounts.Add(new PayeeBankAccountDetail
                            //{
                            //    AccountName = model.AccountDetails.AccountName,
                            //    AccountNumber = model.AccountDetails.AccountNumber,
                            //    AccountType = model.AccountDetails.AccountType,
                            //    BankAddress = model.AccountDetails.BankAddress,
                            //    BankName = model.AccountDetails.BankName,
                            //    IBAN = model.AccountDetails.IBAN,
                            //    IFSC = model.AccountDetails.IFSC,
                            //    Swift = model.AccountDetails.Swift
                            //});
                        }
                        _dbContext.SaveChanges();
                        guid = bankDetails.Id;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return guid;
        }


        internal Guid RemovePayeeBank(Models.PayeeBankViewModel model)
        {
            var guid = Guid.Empty;
            if (model != null)
            {
                try
                {
                    var payeeId = model.Id.ToGuid();
                    var existingPayee = _dbContext.Payees.Where(w => w.Id.Equals(payeeId)).FirstOrDefault();
                    if (existingPayee != null)
                    {
                        var bankId = model.AccountDetails.Id.ToGuid();
                        var existingBank = existingPayee.PayeeBankAccounts.Where(w => w.Id.Equals(bankId)).FirstOrDefault();

                        if (existingBank != null)
                        {
                            existingBank.IsEnabled = false;
                            _dbContext.SaveChanges(); 
                            guid = existingBank.Id;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return guid;
        }
    }
}