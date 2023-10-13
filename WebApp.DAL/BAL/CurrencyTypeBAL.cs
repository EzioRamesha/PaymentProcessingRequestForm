using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.DAL;
using WebApp.DAL.Data;
using WebApp.DAL.Helpers;
using WebApp.DAL.Models;
using WebApp.DAL.Models.ResponseModels;

namespace WebApp.DAL.BAL
{
    public class CurrencyTypeBAL
    {
        private static readonly CurrencyTypeDAL _currencyTypeDAL = new CurrencyTypeDAL();

        public ResponseObject<SaveCurrencyResponse> Save(NewCurrency newCurrency, string userEmail)
        {
            ResponseObject<SaveCurrencyResponse> response;
            using (var _currencyTypeDAL = new CurrencyTypeDAL())
            {
                var id = _currencyTypeDAL.Save(new CurrencyType
                {
                    Name = newCurrency.Name,
                    Code = newCurrency.Code,
                    Description = newCurrency.Description,
                    EuroValue = newCurrency.EuroValue,
                    USDValue = newCurrency.USDValue
                }, userEmail);
                if (id != Guid.Empty)
                {
                    response = new ResponseObject<SaveCurrencyResponse>
                    {
                        ResponseType = "success",
                        Message = "Currency added successfully!"
                    };
                }else
                {
                    response = new ResponseObject<SaveCurrencyResponse>
                    {
                        ResponseType = "error",
                        Message = "Something went wrong while adding the currency!"
                    };
                }
            }
            return response;
        }
        public List<CurrencyType> GetActive()
        {
            return _currencyTypeDAL.List().Where(w => w.IsEnabled).Select(s => new CurrencyType
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Code = s.Code,
                Description = s.Description,
                EuroValue = s.EuroValue,
                USDValue = s.USDValue,
                IsEnabled = s.IsEnabled
            }).ToList();
        }

        public CurrencyType GetById(Guid guid)
        {
            var CurrencyType = _currencyTypeDAL.List().Where(w => w.Id.Equals(guid)).Select(s => new CurrencyType
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Code = s.Code,
                Description = s.Description,
                EuroValue = s.EuroValue,
                USDValue = s.USDValue,
                IsEnabled = s.IsEnabled
            }).FirstOrDefault();
            return CurrencyType;
        }

        public bool Enable(CurrencyType currency)
        {
            var success = false;
            try
            {
                _currencyTypeDAL.ChangeActiveStatus(currency, true);
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }


        public bool Disable(CurrencyType currency)
        {
            var success = false;
            try
            {
                _currencyTypeDAL.ChangeActiveStatus(currency, false);
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }

        public List<CurrencyType> GetAll()
        {
            return _currencyTypeDAL.List()
                                    .OrderBy(o=>o.Name)
                                    .Select(s => new CurrencyType
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Code = s.Code,
                Description = s.Description,
                EuroValue = s.EuroValue,
                USDValue = s.USDValue,
                IsEnabled = s.IsEnabled
            }).ToList();
        }

        public ResponseObject<UpdateCurrencyResponse> Update(UpdateCurrencyViewModel currency, string userEmail)
        {
            ResponseObject<UpdateCurrencyResponse> response;
            using (var _currencyTypeDAL = new CurrencyTypeDAL())
            {
                var currencyIdToSearch = currency.Id.ToGuid();
                var existingCurrency = _currencyTypeDAL.List().Where(w => w.Id.Equals(currencyIdToSearch)).FirstOrDefault();

                if (existingCurrency != null)
                {
                    var id = _currencyTypeDAL.Update(currency);
                    if (!id.IsNullOrEmpty())
                    {
                        response = new ResponseObject<UpdateCurrencyResponse>
                        {
                            ResponseType = "success",
                            Message = "Successfully updated the currency!"
                        };
                    }
                    else
                    {
                        response = new ResponseObject<UpdateCurrencyResponse>
                        {
                            ResponseType = "error",
                            Message = "Something went wrong while updating the currency!"
                        };
                    }
                }
                else
                {
                    // Currency not found in the database
                    response = new ResponseObject<UpdateCurrencyResponse>
                    {
                        ResponseType = "error",
                        Message = "Currency not found!"
                    };
                }

                //if (existingCurrency != null)
                //{
                //    var id = _currencyTypeDAL.Save(new CurrencyType
                //    {
                //        Id = currency.Id,
                //        Name = existingCurrency.Name,
                //        Code = currency.Code,
                //        EuroValue = currency.EuroValue,
                //        USDValue = currency.USDValue,
                //        Description = existingCurrency.Description
                //    }, userEmail);
                //    if (!id.Equals(Guid.Empty))
                //    {
                //        var result = Disable(new CurrencyType {
                //            Id = existingCurrency.Id.ToString()
                //        });
                //        if (result)
                //        {
                //            response = new ResponseObject<UpdateCurrencyResponse>
                //            {
                //                ResponseType = "success",
                //                Message = "Successfully updated the currency!"
                //            };
                //        }
                //        else
                //        {
                //            response = new ResponseObject<UpdateCurrencyResponse>
                //            {
                //                ResponseType = "error",
                //                Message = "Something went wrong while updating the currency!"
                //            };
                //        }
                //    }
                //    else
                //    {
                //        response = new ResponseObject<UpdateCurrencyResponse>
                //        {
                //            ResponseType = "error",
                //            Message = "Something went wrong while updating the currency!"
                //        };
                //    }
                //}
                //else
                //{
                //    // Currency not found in the database
                //    response = new ResponseObject<UpdateCurrencyResponse>
                //    {
                //        ResponseType = "error",
                //        Message = "Currency not found!"
                //    };
                //}
            }
            return response;
        }
    }
}
