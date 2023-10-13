using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.App_Start.Attributes;
using WebApp.DAL;
using WebApp.DAL.BAL;
using WebApp.DAL.Models;
using WebApp.DAL.DAL;
using WebApp.DAL.Helpers;
using WebApp.DAL.Models.ResponseModels;

namespace WebApp.Controllers
{
    [CustomAuthorize("Admin,Operator")]
    public class SettingsController : BaseController
    {
        private static readonly UserBAL _userBAL = new UserBAL();
        private static readonly DepartmentBAL _departmentBAL = new DepartmentBAL();
      
        
        private static readonly DepartmentsAccountBAL _departmentsAccountBAL = new DepartmentsAccountBAL();
        private static readonly GeneralSettingsBAL _generalSettingsBAL = new GeneralSettingsBAL();
        private static readonly PayingEntityBAL _payingEntityBAL = new PayingEntityBAL();
        private static readonly PayeeBAL _payeeBAL = new PayeeBAL();

        private static readonly CountryBAL _countryBAL = new CountryBAL();
        private static readonly CurrencyTypeBAL _currencyTypeBAL = new CurrencyTypeBAL();
        private static readonly PaymentMethodBAL _paymentMethodBAL = new PaymentMethodBAL();
        private static readonly ExpenseTypeBAL _expenseTypeBAL = new ExpenseTypeBAL();
        private static readonly FrequencyTypeBAL _frequencyTypeBAL = new FrequencyTypeBAL();
        private static readonly CancelPPRFReasonTypeBAL _CancelPPRFReasonTypeDAL = new CancelPPRFReasonTypeBAL();
        private static readonly ClosePPRFReasonTypesBAL _ClosePPRFReasonTypesDAL = new ClosePPRFReasonTypesBAL();
        private static readonly RejectTypeBAL _rejectTypeBAL = new RejectTypeBAL();

        #region User
        [CustomAuthorize("Admin")]
        public ActionResult Users()
        {
            return View("~/Views/Settings/Users/Index.cshtml");
        }
        [CustomAuthorize("Admin")]
        public ActionResult GetUsers()
        {
            return Json(_userBAL.GetAllOperators(), JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult CreateUser(NewUserViewModel model)
        {
            ResponseObject<CreateUserResponseModel> response = null;
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                bool emailExists = _userBAL.EmailAlreadyRegistered(model.Email);
                if (!emailExists)
                {
                    var appUser = new DAL.Data.ApplicationUser()
                    {
                        Email = model.Email,
                        UserName = model.Email,
                    };
                    var result = UserManager.CreateAsync(appUser, model.Password).Result;
                    if (result.Succeeded)
                    {
                        var role = "Operator";
                        if (model.Role != null || !model.Name.Equals("admin", StringComparison.OrdinalIgnoreCase))
                        {
                            role = model.Role.Name;
                        }
                        var addToRoleResult = UserManager.AddToRoleAsync(appUser.Id, role).Result;

                        model.ExternalUserId = appUser.Id.ToString();
                        isSuccess = _userBAL.CreateUser(model);
                        if (isSuccess)
                        {
                            response = new ResponseObject<CreateUserResponseModel>
                            {
                                ResponseType = "success",
                                Message = "User created successfully"
                            };
                        }
                        else
                        {
                            result = UserManager.DeleteAsync(appUser).Result;
                            response = new ResponseObject<CreateUserResponseModel>
                            {
                                ResponseType = "error",
                                Message = "Something went wrong while creating the user (1)"
                            };
                        }
                    }
                    else
                    {
                        response = new ResponseObject<CreateUserResponseModel>
                        {
                            ResponseType = "error",
                            Message = "Something went wrong while creating the user (2)"
                        };
                    }
                }
                else
                {
                    response = new ResponseObject<CreateUserResponseModel>()
                    {
                        ResponseType = "error",
                        Message = "Email address already in use."
                    };
                }
            }
            else
            {
                response = new ResponseObject<CreateUserResponseModel>
                {
                    ResponseType = "error",
                    Message = "Invalid user data"
                };
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult UpdateUser(UpdateUserViewModel model)
        {
            ResponseObject<UpdateUserResponseModel> response = null;
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                bool emailExists = _userBAL.EmailAlreadyRegistered(model.Email);
                if (emailExists)
                {
                    isSuccess = _userBAL.UpdateUser(model);
                    if (isSuccess)
                    {
                        response = new ResponseObject<UpdateUserResponseModel>
                        {
                            ResponseType = "success",
                            Message = "User updated successfully"
                        };
                    }
                    else
                    {
                        response = new ResponseObject<UpdateUserResponseModel>
                        {
                            ResponseType = "error",
                            Message = "Something went wrong while trying to update the user details."
                        };
                    }
                }
                else
                {
                    response = new ResponseObject<UpdateUserResponseModel>
                    {
                        ResponseType = "error",
                        Message = "User not found."
                    };
                }
            }
            else
            {
                response = new ResponseObject<UpdateUserResponseModel>
                {
                    ResponseType = "error",
                    Message = "Invalid user data"
                };
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize("Admin")]
        public ActionResult GetUserByEmail(User user)
        {
            var existingUser = _userBAL.FindByEmail(user.Email);
            return Json(existingUser, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult EnableUser(User user)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _userBAL.Enable(user);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult DisableUser(User user)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _userBAL.Disable(user);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult ResetPassword(string userEmail)
        {
            var isSuccess = false;
            var password = string.Empty;
            if (!string.IsNullOrEmpty(userEmail))
            {
                password = _userBAL.ResetPassword(userEmail);
                if (!string.IsNullOrEmpty(password))
                    isSuccess = true;
            }
            return Json(new
            {
                Type = isSuccess ? "success" : "error",
                NewPassword = isSuccess ? password : string.Empty
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region RequestType
        [CustomAuthorize("Admin")]

        public ActionResult RequestType()
        {
            return View("~/Views/Settings/RequestType/Index.cshtml");
        }
        #endregion



        #region Department
        [CustomAuthorize("Admin")]
        public ActionResult CreateDepartment(AddDepartmentViewModel model)
        {
            return Json(_departmentBAL.Save(model), JsonRequestBehavior.AllowGet);
        }


        public ActionResult CreateClosePPRFReasonTypes(WebApp.DAL.Data.ClosePPRFReasonTypes closePPRFReasonTypes)
        {
            return Json(_ClosePPRFReasonTypesDAL.Create(closePPRFReasonTypes), JsonRequestBehavior.AllowGet);
        }


        public ActionResult CreateCancelPPRFReasonTypes(WebApp.DAL.Data.CancelPPRFReasonTypes cancelPPRFReasonTypes)
        {
            return Json(_CancelPPRFReasonTypeDAL.Create(cancelPPRFReasonTypes), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateRejectType(RejectType rejectType)
        {
            return Json(_rejectTypeBAL.Create(rejectType), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateDepartment(UpdateDepartmentViewModel model)
        {
            return Json(_departmentBAL.Update(model), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ClosePPRFReasonTypesupdate(WebApp.DAL.Data.ClosePPRFReasonTypes closePPRFReasonTypes)
        {
            return Json(_ClosePPRFReasonTypesDAL.Update(closePPRFReasonTypes), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CancelPPRFReasonTypesupdate(WebApp.DAL.Data.CancelPPRFReasonTypes cancelPPRFReasonTypes)
        {
            return Json(_CancelPPRFReasonTypeDAL.Update(cancelPPRFReasonTypes), JsonRequestBehavior.AllowGet);
        }

        public ActionResult RejectTypeupdate(RejectType rejectType)
        {
            return Json(_rejectTypeBAL.Update(rejectType), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CustomAuthorize("Admin")]
        public ActionResult EnableRejectType(RejectType rejectType)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _rejectTypeBAL.Enable(rejectType);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CustomAuthorize("Admin")]
        public ActionResult DisableRejectType(RejectType rejectType)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _rejectTypeBAL.Disable(rejectType);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CustomAuthorize("Admin")]
        public ActionResult CancelPPRFReasonTypesEnable(WebApp.DAL.Data.CancelPPRFReasonTypes cancelPPRFReasonTypes)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _CancelPPRFReasonTypeDAL.Enable(cancelPPRFReasonTypes);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CustomAuthorize("Admin")]
        public ActionResult CancelPPRFReasonTypesDisable(WebApp.DAL.Data.CancelPPRFReasonTypes cancelPPRFReasonTypes)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _CancelPPRFReasonTypeDAL.Disable(cancelPPRFReasonTypes);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CustomAuthorize("Admin")]
        public ActionResult ClosePPRFReasonTypesEnable(WebApp.DAL.Data.ClosePPRFReasonTypes closePPRFReasonTypes)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _ClosePPRFReasonTypesDAL.Enable(closePPRFReasonTypes);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CustomAuthorize("Admin")]
        public ActionResult ClosePPRFReasonTypesDisable(WebApp.DAL.Data.ClosePPRFReasonTypes closePPRFReasonTypes)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _ClosePPRFReasonTypesDAL.Disable(closePPRFReasonTypes);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        public ActionResult createDepartmentsAccount(AddDepartmentsAccountViewModel model)
        {
            return Json(_departmentsAccountBAL.Save(model), JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateDepartmentsAccount(UpdateDepartmentsAccountViewModel model)
        {
            return Json(_departmentsAccountBAL.Update(model), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Department()
        {
            return View("~/Views/Settings/Department/Index.cshtml");
        }

        public ActionResult DepartmentsAccount()
        {
            return View("~/Views/Settings/DepartmentsAccounts/Index.cshtml");
        }

        [HttpPost]
        [CustomAuthorize("Admin")]
        public ActionResult EnableDepartment(Department department)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _departmentBAL.Enable(department);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CustomAuthorize("Admin")]
        public ActionResult DisableDepartment(Department department)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
               isSuccess = _departmentBAL.Disable(department);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [CustomAuthorize("Admin")]
        public ActionResult EnableDepartmentsAccount(DepartmentsAccount departmentsAccount)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _departmentsAccountBAL.Enable(departmentsAccount);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CustomAuthorize("Admin")]
        public ActionResult DisableDepartmentsAccount(DepartmentsAccount departmentsAccount)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _departmentsAccountBAL.Disable(departmentsAccount);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDepartments()
        {
            return Json(_departmentBAL.GetAll(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDepartmentsAccount()
        {
            return Json(_departmentsAccountBAL.GetAll(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCancelPPRFReasonTypes()
        {
            return Json(_CancelPPRFReasonTypeDAL.GetAllRejectTypes(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetClosePPRFReasonTypes()
        {
            return Json(_ClosePPRFReasonTypesDAL.GetAllRejectTypes(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRejectTypes()
        {
            return Json(_rejectTypeBAL.GetAllRejectTypes(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDepartmentsAccountByDeptId(DepartmentsAccount DepartmentId)
        {
            return Json(_departmentsAccountBAL.GetDepartmentsAccountDetails(DepartmentId), JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Paying Entity
        [CustomAuthorize("Admin")]
        public ActionResult PayingEntities()
        {
            return View("~/Views/Settings/PayingEntities/Index.cshtml");
        }
        [CustomAuthorize("Admin")]
        public ActionResult GetPayingEntities(ManagePayingEntitiesViewModel model)
        {
            return Json(_payingEntityBAL.GetAll(model), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetActivePayingEntities()
        {
            return Json(_payingEntityBAL.GetActive(), JsonRequestBehavior.AllowGet);
        }


        [CustomAuthorize("Admin")]
        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult CreatePayingEntity(AddPayingEntityViewModel model)
        {
            return Json(_payingEntityBAL.Save(model), JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult UpdatePayingEntity(UpdatePayingEntityViewModel model)
        {
            return Json(_payingEntityBAL.Update(model), JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult EnablePayingEntity(PayingEntity payingEntity)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _payingEntityBAL.Enable(payingEntity);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult DisablePayingEntity(PayingEntity payingEntity)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _payingEntityBAL.Disable(payingEntity);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize("Admin")]
        [HttpGet]
        public ActionResult ManageEntityAmountRange(string payingEntityId)
        {
            ResponseObject<ViewEntityAmountRange> response = null;
            if (!string.IsNullOrEmpty(payingEntityId))
            {
                ViewEntityAmountRange config = _payingEntityBAL.GetRangeConfigurationFor(payingEntityId);
                if (config != null)
                {
                    response = new ResponseObject<ViewEntityAmountRange>
                    {
                        Data = config,
                        Message = "Range configuration found.",
                        ResponseType = "success"
                    };
                }
                else
                {
                    response = new ResponseObject<ViewEntityAmountRange>
                    {
                        Data = config,
                        Message = "Range configuration could not be located.",
                        ResponseType = "error"
                    };
                }
            }
            return Content(JsonConvert.SerializeObject(response));
        }

        [HttpPost]
        public ActionResult AddEntityAmountRange(AddRange range)
        {
            ResponseObject<AddRange> response = null;
            if (ModelState.IsValid)
            {
                response = _payingEntityBAL.AddEntityAmountRange(range);
            }
            else
            {
                response = new ResponseObject<AddRange>
                {
                    Message = "Invalid data provided",
                    ResponseType = "error"
                };
            }
            return Content(JsonConvert.SerializeObject(response));
        }
        [HttpPost]
        public ActionResult AddEntityRangeEmail(AddEmail rangeEmail)
        {
            ResponseObject<AddEmail> response = null;
            if (ModelState.IsValid)
            {
                response = _payingEntityBAL.AddEmailForRange(rangeEmail);
            }
            else
            {
                response = new ResponseObject<AddEmail>
                {
                    Message = "Invalid data provided",
                    ResponseType = "error"
                };
            }
            return Content(JsonConvert.SerializeObject(response));
        }
        [HttpGet]
        public ActionResult GetRangeDetails(string rangeId)
        {
            ResponseObject<RangeEmails> response = null;
            if(!string.IsNullOrEmpty(rangeId) && !string.IsNullOrWhiteSpace(rangeId))
            {
                response = _payingEntityBAL.GetRangeDetailsForRangeId(rangeId);
            }
            else
            {
                response = new ResponseObject<RangeEmails>
                {
                    Message = "Invalid data provided",
                    ResponseType = "error"
                };
            }
            return Content(JsonConvert.SerializeObject(response));
        }
        [HttpDelete]
        public ActionResult DeleteEmailFromRange(DeleteEmailFromRange model)
        {
            ResponseObject<DeleteEmailFromRange> response = null;
            if (ModelState.IsValid)
            {
                response = _payingEntityBAL.DeleteEmailFromRange(model);
            } else
            {
                response = new ResponseObject<DAL.Models.DeleteEmailFromRange>
                {
                    Message = "Invalid data provided",
                    ResponseType = "error"
                };
            }
            return Content(JsonConvert.SerializeObject(response));
        }
        #endregion


        #region Payee
        public ActionResult Payees()
        {
            return View("~/Views/Settings/Payees/Index.cshtml");
        }
        public ActionResult GetPayees(ManagePayeeViewModel model)
        {
            return Json(_payeeBAL.GetAll(model), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPayeesByName(ManagePayeeViewModel model,string PayeeName)
        {
            return Json(_payeeBAL.GetAllFilterByName(model,PayeeName), JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        public ActionResult GetPayeeAccountDetails(PayeeModel payee)
        {
            return Json(_payeeBAL.GetAccountDetails(payee), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetDepartmentDetails(PayingEntity PayingEntitiesId)
        {
            return Json(_departmentBAL.GetDepartmentDetails(PayingEntitiesId.Id), JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult CreatePayee(NewPayeeViewModel model)
        {
            return Json(_payeeBAL.CreatePayee(model), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdatePayee(UpdatePayeeViewModel model)
        {
            return Json(_payeeBAL.UpdatePayee(model), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult EnablePayee(PayeeModel payee)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _payeeBAL.Enable(payee);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DisablePayee(PayeeModel payee)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _payeeBAL.Disable(payee);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdatePayeeBankDetails(PayeeDetailsViewModel model)
        {
            return Json(_payeeBAL.UpdatePayeeAccountDetails(model), JsonRequestBehavior.AllowGet);
            //return null;
        }

        [HttpPost]
        public ActionResult AddOrUpdatePayeeBank(PayeeBankViewModel model)
        {
            //throw new NotImplementedException();
            return Json(_payeeBAL.AddOrUpdatePayeeBank(model), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult RemovePayeeBank(PayeeBankViewModel model)
        {
            //throw new NotImplementedException();
            return Json(_payeeBAL.RemovePayeeBank(model), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetActivePayees()
        {
            return Json(_payeeBAL.GetActive(), JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Country
        [CustomAuthorize("Admin")]
        public ActionResult Countries()
        {
            return View("~/Views/Settings/Countries/Index.cshtml");
        }

        [CustomAuthorize("Admin")]
        public ActionResult GetCountries()
        {
            return Json(_countryBAL.GetAll(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetActiveCountries()
        {
            return Json(_countryBAL.GetActive(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CustomAuthorize("Admin")]
        public ActionResult EnableCountry(Country country)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _countryBAL.Enable(country);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CustomAuthorize("Admin")]
        public ActionResult DisableCountry(Country country)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _countryBAL.Disable(country);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Currency
        [CustomAuthorize("Admin")]
        public ActionResult Currency()
        {
            return View("~/Views/Settings/Currency/Index.cshtml");
        }
        [HttpPost]
        public ActionResult CreateCurrency(NewCurrency model)
        {
            return Json(_currencyTypeBAL.Save(model, User.Identity.Name), JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize("Admin")]
        public ActionResult GetCurrencies()
        {
            return Json(_currencyTypeBAL.GetAll(), JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult EnableCurrency(CurrencyType currency)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _currencyTypeBAL.Enable(currency);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult DisableCurrency(CurrencyType currency)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _currencyTypeBAL.Disable(currency);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }



        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult UpdateCurrency(UpdateCurrencyViewModel currency)
        {
            ResponseObject<UpdateCurrencyResponse> responseObject;
            if (ModelState.IsValid)
            {
                responseObject = _currencyTypeBAL.Update(currency, User.Identity.Name);
            }
            else
            {
                responseObject = new ResponseObject<UpdateCurrencyResponse>
                {
                    ResponseType = "error",
                    Message = "Incorrect data provided"
                };
            }
            return Json(responseObject, JsonRequestBehavior.AllowGet);
        }
        #endregion







        #region PaymentMethod
        [CustomAuthorize("Admin")]
        public ActionResult PaymentMethods()
        {
            return View("~/Views/Settings/PaymentMethods/Index.cshtml");
        }
        [CustomAuthorize("Admin")]
        public ActionResult GetPaymentMethods()
        {
            return Json(_paymentMethodBAL.GetAll(), JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult EnablePaymentMethod(PaymentMethod paymentMethod)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _paymentMethodBAL.Enable(paymentMethod);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult DisablePaymentMethod(PaymentMethod paymentMethod)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _paymentMethodBAL.Disable(paymentMethod);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }


        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult CreatePaymentMethod(PaymentMethod paymentMethod)
        {
            ResponseObject<CreatePaymentMethodResponse> responseObject;
            if (ModelState.IsValid)
            {
                responseObject = _paymentMethodBAL.Create(paymentMethod);
            }
            else
            {
                responseObject = new ResponseObject<CreatePaymentMethodResponse>
                {
                    ResponseType = "error",
                    Message = "Incorrect data provided"
                };
            }
            return Json(responseObject, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult UpdatePaymentMethod(PaymentMethod paymentMethod)
        {
            ResponseObject<UpdatePaymentMethodResponse> responseObject;
            if (ModelState.IsValid)
            {
                responseObject = _paymentMethodBAL.Update(paymentMethod);
            }
            else
            {
                responseObject = new ResponseObject<UpdatePaymentMethodResponse>
                {
                    ResponseType = "error",
                    Message = "Incorrect data provided"
                };
            }
            return Json(responseObject, JsonRequestBehavior.AllowGet);
        }
        #endregion



        #region ExpenseTypes
        [CustomAuthorize("Admin")]
        public ActionResult ExpenseTypes()
        {
            return View("~/Views/Settings/ExpenseTypes/Index.cshtml");
        }
        [CustomAuthorize("Admin")]
        public ActionResult GetExpenseTypes()
        {
            return Json(_expenseTypeBAL.GetAll(), JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult CreateExpenseType(ExpenseType expenseType)
        {
            ResponseObject<CreateExpenseTypeResponse> response = null;
            if (ModelState.IsValid)
                response = _expenseTypeBAL.Create(expenseType);
            else
                response = new ResponseObject<CreateExpenseTypeResponse>
                {
                    ResponseType = "error",
                    Message = "Invalid data given"
                };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult UpdateExpenseType(ExpenseType expenseType)
        {
            ResponseObject<UpdateExpenseTypeResponse> response = null;
            if (ModelState.IsValid)
                response = _expenseTypeBAL.Update(expenseType);
            else
                response = new ResponseObject<UpdateExpenseTypeResponse>
                {
                    ResponseType = "error",
                    Message = "Invalid data given"
                };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult EnableExpenseType(ExpenseType expenseType)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _expenseTypeBAL.Enable(expenseType);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult DisableExpenseType(ExpenseType expenseType)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _expenseTypeBAL.Disable(expenseType);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }
        #endregion



        #region Frequency Type
        [CustomAuthorize("Admin")]
        public ActionResult FrequencyTypes()
        {
            return View("~/Views/Settings/FrequencyTypes/index.cshtml");
        }
        [CustomAuthorize("Admin")]
        public ActionResult GetFrequencyTypes()
        {

            return Json(_frequencyTypeBAL.GetAll(), JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult CreateFrequencyType(FrequencyType frequencyType)
        {
            ResponseObject<CreateFrequencyTypeResponse> response = null;
            if (ModelState.IsValid)
                response = _frequencyTypeBAL.Create(frequencyType);
            else
                response = new ResponseObject<CreateFrequencyTypeResponse>
                {
                    ResponseType = "error",
                    Message = "Invalid data given"
                };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult UpdateFrequencyType(FrequencyType frequencyType)
        {
            ResponseObject<UpdateFrequencyTypeResponse> response = null;
            if (ModelState.IsValid)
                response = _frequencyTypeBAL.Update(frequencyType);
            else
                response = new ResponseObject<UpdateFrequencyTypeResponse>
                {
                    ResponseType = "error",
                    Message = "Invalid data given"
                };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult EnableFrequencyType(FrequencyType frequencyType)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _frequencyTypeBAL.Enable(frequencyType);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorize("Admin")]
        [HttpPost]
        public ActionResult DisableFrequencyType(FrequencyType frequencyType)
        {
            var isSuccess = false;
            if (ModelState.IsValid)
            {
                isSuccess = _frequencyTypeBAL.Disable(frequencyType);
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult GetActiveAppGroups()
        {
            return Json(_generalSettingsBAL.GetActiveUserGroups(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetActivePermissions()
        {
            return Json(_generalSettingsBAL.GetActiveUserPermissions(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetActiveRoles()
        {
            return Json(_generalSettingsBAL.GetAllRolesExceptAdmin(), JsonRequestBehavior.AllowGet);
        }
    }
}