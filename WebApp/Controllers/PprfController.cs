using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.App_Start.Attributes;
using WebApp.DAL.BAL;
using WebApp.DAL.Models;
using WebApp.DAL.Models.ResponseModels;

using WebApp.DAL;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    [CustomAuthorize("Admin,Operator")]
    public class PprfController : BaseController
    {
        private static readonly PayingEntityBAL _payingEntityBAL = new PayingEntityBAL();
        private static readonly CountryBAL _countryBAL = new CountryBAL();
        private static readonly DepartmentBAL _departmentBAL = new DepartmentBAL();
        private static readonly DepartmentsAccountBAL _departmentsAccountBAL = new DepartmentsAccountBAL();
        private static readonly FrequencyTypeBAL _frequencyTypeBAL = new FrequencyTypeBAL();
        private static readonly PayeeBAL _payeeBAL = new PayeeBAL();
        private static readonly PaymentMethodBAL _paymentMethodBAL = new PaymentMethodBAL();
        private static readonly ExpenseTypeBAL _expenseTypeBAL = new ExpenseTypeBAL();
        private static readonly CurrencyTypeBAL _currencyTypeBAL = new CurrencyTypeBAL();
        private static readonly TaxTypeBAL _taxTypeBAL = new TaxTypeBAL();
        private static readonly UserBAL _userBAL = new UserBAL();
        private static readonly BudgetOrderBAL _budgetOrderBAL = new BudgetOrderBAL();
        private static readonly RejectTypeBAL _rejectTypeBAL = new RejectTypeBAL();

        private static readonly RequestFormBAL _requestFormBAL = new RequestFormBAL();
        
        // GET: Pprf
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult New()
        {
            return View();
        }

        public ActionResult GetInitialData()
        {
            PaymentRequestFormViewModel model = new PaymentRequestFormViewModel
            {
                PayingEntities = _payingEntityBAL.GetActive(),
                Countries = _countryBAL.GetActive(),
               // Departments = _departmentBAL.GetActive(),
                DepartmentsAccounts = _departmentsAccountBAL.GetActive(),
                FrequencyTypes = _frequencyTypeBAL.GetActive(),
                PaymentMethods = _paymentMethodBAL.GetActive(),
                ExpenseTypes = _expenseTypeBAL.GetActive(),
                Payees = _payeeBAL.GetActive(),
                CurrencyTypes = _currencyTypeBAL.GetActive(),
                TaxTypes = _taxTypeBAL.GetActive(),
                Operators = _userBAL.GetApplicationGroups(),
                BudgetOrders = _budgetOrderBAL.GetBudgetOrderList(DateTime.Now)

            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBudgetPayeeData(string PayeeBankAccountDetailId)
        {
            PayeeModel payee = _payeeBAL.GetPayeeInfo(PayeeBankAccountDetailId);
            return Json(payee, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBurgetOrderListByDate(DateTime PPRFDate)
        {
            List<BudgetOrder> budgetList = _budgetOrderBAL.GetBudgetOrderList(PPRFDate);
            return Json(budgetList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public async Task<ActionResult> SaveNewRequest(SavePaymentProcessingRequestFormViewModel model)
        {

            var user = _userBAL.FindByEmail(User.Identity.Name);
            if (user != null)
            {
                model.CreatedByEmail = user.Email;
            }
            var response = await _requestFormBAL.SaveForm(model);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public  ActionResult SaveDraft(SavePaymentProcessingRequestFormViewModel model)
        {

            var user = _userBAL.FindByEmail(User.Identity.Name);
            if (user != null)
            {
                model.CreatedByEmail = user.Email;
            }
            var response =  _requestFormBAL.SaveDraftForm(model);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [MyValidateAntiForgeryToken]
        public async Task<ActionResult> DraftUpdate(SavePaymentProcessingRequestFormViewModel model)
        {

            var user = _userBAL.FindByEmail(User.Identity.Name);
            if (user != null)
            {
                model.CreatedByEmail = user.Email;
            }
            var response = await _requestFormBAL.UpdateDraftForm(model);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [MyValidateAntiForgeryToken]
        public ContentResult GetOriginatorRequestById(string requestId)
        {
            var user = _userBAL.FindByEmail(User.Identity.Name);
            var result = new RequestFormDetails();
            if (user != null)
            {
                var email = user.Email;
                result = _requestFormBAL.GetOriginatorFormByFormId(requestId, email);
            }

            return Content(JsonConvert.SerializeObject(result));
        }

        [HttpGet]
        [MyValidateAntiForgeryToken]
        public ContentResult GetApproverRequestById(string requestId)
        {
            var user = _userBAL.FindByEmail(User.Identity.Name);
            var result = new RequestFormDetails();
            if (user != null)
            {
                result = _requestFormBAL.GetApproverFormByFormId(requestId, user.Email);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        [HttpGet]
        [MyValidateAntiForgeryToken]
        public ContentResult GetApprovedRequestDetails(string requestId)
        {
            var result = new RequestFormDetails();
            if (!string.IsNullOrEmpty(requestId))
            {
                result = _requestFormBAL.GetApprovedRequest(requestId);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        [HttpGet]
        [MyValidateAntiForgeryToken]
        public ContentResult GetRequestById(string requestId)
        {
            var result = new RequestFormDetails();
            if (!string.IsNullOrEmpty(requestId))
            {
                result = _requestFormBAL.GetUserRequestById(requestId);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        [HttpGet]
        [MyValidateAntiForgeryToken]
        public ContentResult GetBudgetSpendingInfo(string BudgetrequestId)
        {
            var result = new BudgetSpentInfo();
            if (!string.IsNullOrEmpty(BudgetrequestId))
            {
                result = _requestFormBAL.GetBudgetSpentInfoById(BudgetrequestId);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        [HttpGet]
        [MyValidateAntiForgeryToken]
        public ContentResult GetClosedRequestDetails(string requestId)
        {
            var result = new RequestFormDetails();
            if (!string.IsNullOrEmpty(requestId))
            {
                //result = _requestFormBAL.GetClosedRequest(requestForm.Id);
                result = _requestFormBAL.GetClosedOrRejectedRequest(requestId);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        [AllowAnonymous]
        public ActionResult AskAQuestion([System.Web.Http.FromUri]string token, [System.Web.Http.FromUri]string v)
        {
            var model = new ApproveRejectRequestModel
            {
                token = token,
                v = v
            };
            string retVal = string.Empty;
            var approval = _requestFormBAL.GetApprovalByToken(token, v);

            if (approval == null)
            {
                retVal = "Request not found";
                //return Json("Request not found", JsonRequestBehavior.AllowGet);
            }
            else if (approval.Data.ApprovalStatus.Equals("Pending") && approval.Data.QueryCount < 2)
            {
                if (approval.Data.HasPendingQuestions)
                    retVal = "You have already asked a question. Please let the originator answer it first.";
                else
                    return View(model);
            }
            else
            {
                retVal = "Request already processed";
                //return Json("Request already processed", JsonRequestBehavior.AllowGet);
            }
            return Content(retVal);
        }

        [AllowAnonymous]
        public async Task<ActionResult> SaveQuestion([System.Web.Http.FromUri]string token, [System.Web.Http.FromUri]string v, [System.Web.Http.FromUri]string question)
        {
            var model = new ApproveRejectRequestModel
            {
                token = token,
                v = v
            };
            var approval = _requestFormBAL.GetApprovalByToken(token, v);
            string retVal = string.Empty;
            if (approval == null)
            {
                retVal = "Request not found";
                //return Json("Request not found", JsonRequestBehavior.AllowGet);
            }
            else if (approval.Data.ApprovalStatus.Equals("Pending") && approval.Data.QueryCount < 2)
            {
                var form = _requestFormBAL.GetFormByToken(token, v);
                if (form != null)
                {
                    var response = _requestFormBAL.ApproverAskQuestion(form.Id, question, approval.Data.ApproverEmail);
                    retVal = response.Message;
                    if (response.ResponseType.Equals("success", StringComparison.OrdinalIgnoreCase))
                        await _requestFormBAL.SendQuestionAskedEmail(approval.Data.ApproverEmail, form.Id, question);
                }
                else
                    retVal = "Request not found";
                    //return Json("Request not found", JsonRequestBehavior.AllowGet);
            }
            else
            {
                retVal = "Request already processed";
                //return Json("Request already processed", JsonRequestBehavior.AllowGet);
            }
            return Content(retVal);
        }

        [AllowAnonymous]
        public async Task<ActionResult> GetClarification([System.Web.Http.FromUri]string token, [System.Web.Http.FromUri]string v)
        {
            var returnObject = await _requestFormBAL.AskClarificationBy(token, v);
            if (returnObject.ResponseType.Equals("success", StringComparison.OrdinalIgnoreCase))
            {
                return View("~/Views/Pprf/GetClarification.cshtml", returnObject);
            }
            return Content(returnObject.Message);
        }


        [HttpPost]
        [MyValidateAntiForgeryToken]
        public async Task<ActionResult> AskClarification(RequestForm requestForm)
        {
            var returnObject = false;
            if (requestForm != null)
            {
                returnObject = await _requestFormBAL.AskClarificationFor(requestForm.Id, User.Identity.Name);
            }
            return Json(returnObject, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public async Task<ActionResult> GiveClarification(GiveClarificationModel model)
        {
            ResponseObject<UpdateClarificationResponse> returnObject;
            if (ModelState.IsValid)
            {
                returnObject = await _requestFormBAL.UpdateClarification(model, User.Identity.Name);
            }
            else
            {
                returnObject = new ResponseObject<UpdateClarificationResponse>
                {
                    ResponseType = "error",
                    Message = "invalid submission made"
                };
            }
            return Json(returnObject, JsonRequestBehavior.AllowGet);
        }

        [MyValidateAntiForgeryToken]
        public async Task<ActionResult> ReSendApproverEmail(string requestId,string Remark,string Urjent)
        {
            ResponseObject<SendEmailResponse> returnObject = null;
            if (ModelState.IsValid)
            {
                var form = _requestFormBAL.GetFormById(requestId);
                if (form != null)
                {
                    if (form.Originator.Email.Equals(User.Identity.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        returnObject = await _requestFormBAL.SendApproverEmail(requestId, User.Identity.Name, Remark, Urjent);
                    }
                    else
                    {
                        returnObject = new ResponseObject<SendEmailResponse>
                        {
                            ResponseType = "error",
                            Message = "You cannot send email for this request"
                        };
                    }
                }
                else
                {
                    returnObject = new ResponseObject<SendEmailResponse>
                    {
                        ResponseType = "error",
                        Message = "Request not found"
                    };
                }
            }
            else
            {
                returnObject = new ResponseObject<SendEmailResponse>
                {
                    ResponseType = "error",
                    Message = "invalid request"
                };
            }
            return Json(returnObject, JsonRequestBehavior.AllowGet);
        }

        [MyValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> ApproveRequest(RequestForm requestForm)
        {
            var returnObject = false;
            if (requestForm != null)
            {
                returnObject = _requestFormBAL.ApproveRequest(requestForm.Id, User.Identity.Name);
                if (returnObject)
                {
                    var form = _requestFormBAL.GetFormById(requestForm.Id);
                    if (form != null)
                    {
                        if (form.Status.Equals("pending", StringComparison.OrdinalIgnoreCase))
                        {
                            await _requestFormBAL.SendApproverEmail(requestForm.Id, User.Identity.Name,"","");
                            await _requestFormBAL.SendOriginatorNotification(requestForm);
                        }
                        else if (form.Status.Equals("approved", StringComparison.OrdinalIgnoreCase))
                        {
                            await _requestFormBAL.SendPPRFApprovedEmail(form);
                        }
                    }
                }
            }
            return Json(returnObject, JsonRequestBehavior.AllowGet);
        }

        [MyValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> RejectRequest(RequestForm requestForm, string reason,string remark)
        {
            var returnObject = false;
            if (requestForm != null)
            {
                reason = Request.Form["reason"].ToString();
                returnObject = _requestFormBAL.RejectRequest(requestForm.Id, reason, User.Identity.Name, remark);
                if (returnObject)
                {
                    var form = _requestFormBAL.GetFormById(requestForm.Id);
                    await _requestFormBAL.SendPPRFRejectedEmail(requestForm, remark);
                }
            }
            return Json(returnObject, JsonRequestBehavior.AllowGet);
        }



        [AllowAnonymous]
        public ActionResult Approve([System.Web.Http.FromUri]string token, [System.Web.Http.FromUri]string v)
        {
            var model = new ApproveRejectRequestModel
            {
                token = token,
                v = v
            };
            var response = _requestFormBAL.GetApprovalByToken(token, v);
            if (response.ResponseType.Equals("error", StringComparison.OrdinalIgnoreCase))
            {
                return Json(response.Message, JsonRequestBehavior.AllowGet);
            }
            else if (response.Data.ApprovalStatus.Equals("Pending"))
            {
                return View(model);
            }
            else
            {
                return Json("Request already processed", JsonRequestBehavior.AllowGet);
            }
        }

        //[AllowAnonymous]
        //public ActionResult Approve([System.Web.Http.FromUri]string token, [System.Web.Http.FromUri]string v)
        //{
        //    var returnValue = _requestFormBAL.Approve(token, v, string.Empty);
        //    if (returnValue.ResponseType.Equals("success", StringComparison.OrdinalIgnoreCase))
        //    {
        //        var form = _requestFormBAL.GetFormByToken(token, v);
        //        if (form.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
        //        {
        //            _requestFormBAL.SendApproverEmail(form, User.Identity.Name);
        //            _requestFormBAL.SendOriginatorNotification(form);
        //        }
        //        else if (form.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
        //            _requestFormBAL.SendPPRFApprovedEmail(form);
        //        return View("~/Views/Pprf/Approved.cshtml", returnValue);
        //    }
        //    //return View("~/Views/Pprf/Approve-Failed.cshtml", returnValue);
        //    return Content(returnValue.Message);
        //    //return Json(returnValue, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Approve([System.Web.Http.FromUri]string token, [System.Web.Http.FromUri]string v, [System.Web.Http.FromUri]string remarks)
        {
            var returnValue = _requestFormBAL.Approve(token, v, remarks);
            if (returnValue.ResponseType.Equals("success", StringComparison.OrdinalIgnoreCase))
            {
                var form = _requestFormBAL.GetFormByToken(token, v);
                //if(form.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                //{
                //    return View("~/Views/Pprf/AlreadyApproved.cshtml", returnValue);
                //}
                if (form.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                {
                    await _requestFormBAL.SendApproverEmail(form.Id, User.Identity.Name,"","");
                    await _requestFormBAL.SendOriginatorNotification(form);
                }
                else if (form.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                    await _requestFormBAL.SendPPRFApprovedEmail(form);
                return View("~/Views/Pprf/Approved.cshtml", returnValue);
            }
            return Content(returnValue.Message);
        }

        [AllowAnonymous]
        public ActionResult Reject([System.Web.Http.FromUri]string token, [System.Web.Http.FromUri]string v)
        
        {
            var model = new ApproveRejectRequestModel
            {
                token = token,
                v = v
            };

            var approval = _requestFormBAL.GetApprovalByToken(token, v);
            var reqTypes = _rejectTypeBAL.GetAllRejectTypes();
            SelectList list = new SelectList(reqTypes, "Description", "Description");
            ViewBag.RejectReasons = list;
           // model.rejectsTypes = reqTypes;
           
            if (approval == null)
            {
                return Json("Request not found", JsonRequestBehavior.AllowGet);
            }
            else if (approval.Data.ApprovalStatus.Equals("Pending"))
            {
                return View(model);
            }
            else
            {
                return Json("Request already processed", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Permission("Close PO/PPRF")]
        [MyValidateAntiForgeryToken]
        public ActionResult CloseRequest(RequestForm requestForm, string CloseReason, string CloseRemark)
        {
            ResponseObject<CloseRequestResponse> response = null;
            var user = _userBAL.FindByEmail(User.Identity.Name);
            if (user != null)
            {
                response = _requestFormBAL.CloseRequest(requestForm, user,  CloseReason,  CloseRemark);
            }
            return Content(JsonConvert.SerializeObject(response));
        }
        public ActionResult SaveNotesRequest(RequestForm requestForm, string Notes)
        {
            ResponseObject<NotesResponse> response = null;
            var user = _userBAL.FindByEmail(User.Identity.Name);
            if (user != null)
            {

                response = _requestFormBAL.NotesUpdate(requestForm, user, Notes);
            }
            return Content(JsonConvert.SerializeObject(response));
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Reject(string token, string v, string reason,string remark)
        {
            var returnValue = _requestFormBAL.Reject(token, v, reason, remark);
            if (returnValue.ResponseType.Equals("success", StringComparison.OrdinalIgnoreCase))
            {
                var form = _requestFormBAL.GetFormByToken(token, v);
                await _requestFormBAL.SendPPRFRejectedEmail(form, remark);
                return View("~/Views/Pprf/Rejected.cshtml", returnValue);
            }
            return Content(returnValue.Message);
            //return View("~/Views/Pprf/Reject-Failed.cshtml", returnValue);
        }
        
        [HttpGet]
        //[MyValidateAntiForgeryToken]
        public ActionResult GetDocumentFor(string Id)
        {
            FileDownloadModel returnObject = _requestFormBAL.GetFileForRequestId(Id);
            if (returnObject == null)
                return Json(null, JsonRequestBehavior.AllowGet);
            return File(returnObject.FileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, returnObject.FileName);
        }

        [HttpGet]
        public ActionResult GetDocumentByDocId(string docID)
        {
            FileDownloadModel returnObject = _requestFormBAL.GetFileByDocID(docID);
            if (returnObject == null)
                return Json(null, JsonRequestBehavior.AllowGet);
            return File(returnObject.FileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, returnObject.FileName);
        }

        [HttpGet]
        public ActionResult PrintRequest(string requestId)
        {
            var result = new RequestFormDetails();
            //if (requestForm != null && !string.IsNullOrEmpty(requestForm.Id))
            if (!string.IsNullOrEmpty(requestId))
            {
                result = _requestFormBAL.GetUserRequestById(requestId);
                if (result != null)
                    return View(result);
            }
            //return View("~/Views/Pprf/PrintRequest.cshtml", JsonConvert.SerializeObject(result));
            return View("~/Views/Error/PPRFRequestNotFound.cshtml");
        }

        public async Task<ActionResult> UpdatePPRF(UpdateRequestViewModel model)
        {
            ResponseObject<UpdateRequestFormResponse> response = null;
            string type = "error", message="Something went wrong";
            if (!string.IsNullOrEmpty(model.RequestId))
            {
                var request = _requestFormBAL.GetFormById(model.RequestId);
                if (request != null)
                {
                    if (request.Status.Equals("pending", StringComparison.OrdinalIgnoreCase))
                        response = await _requestFormBAL.UpdateRequest(model);
                     
                    else
                    {
                        if (request.Status.Equals("Draft", StringComparison.OrdinalIgnoreCase))
                            response = await _requestFormBAL.UpdateRequest(model);
                        else
                        {
                            type = "error";
                            message = "PPRF/PO already approved";
                        }
                       
                    }
                }
                else
                {
                    type = "error";
                    message = "PPRF/PO not found";
                }
            }
            else
            {
                type = "error";
                message = "Invalid data provided";
            }
            if (response == null)
                response = new ResponseObject<UpdateRequestFormResponse>
                {
                    ResponseType = type,
                    Message = message
                };

            return Content(JsonConvert.SerializeObject(response));
        }

        public ActionResult GetOperators()
        {
            return Json(_userBAL.GetApplicationGroups(), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> AskQuestion(string RequestId, string Question)
        {
            var response = _requestFormBAL.ApproverAskQuestion(RequestId, Question, User.Identity.Name);
            if (response != null && response.ResponseType.Equals("success", StringComparison.OrdinalIgnoreCase))
            {
                await _requestFormBAL.SendQuestionAskedEmail(User.Identity.Name, RequestId, Question);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SaveAnswer(string QuestionId, string Answer)
        {
            var originatorEmail = User.Identity.Name;
            var response = _requestFormBAL.SaveAnswer(QuestionId, Answer, originatorEmail);
            if (response != null && response.ResponseType.Equals("success", StringComparison.OrdinalIgnoreCase))
            {
                RequestForm requestForm = _requestFormBAL.GetFormByQuestionId(QuestionId);
                await _requestFormBAL.SendApproverEmail(requestForm.Id, originatorEmail,"","");
                //_requestFormBAL.SendQueryAnsweredEmail(QuestionId, Answer);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [MyValidateAntiForgeryToken]
        public ContentResult GetRequestQueries(RequestForm requestForm)
        {
            //var user = _userBAL.FindByEmail(User.Identity.Name);
            ResponseObject<List<QuestionAnswer>> response = null;

            //if (user != null)
            //{
                //var email = user.Email;
                //result = _requestFormBAL.GetOriginatorFormByFormId(requestForm.Id, email);
                response = _requestFormBAL.GetRequestQueries(requestForm.Id);
            //}

            return Content(JsonConvert.SerializeObject(response));
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ContentResult CancelRequest(string requestId,string CloseReason,string CloseRemark)
        {
            ResponseObject<CancelRequestResponse> response = null;
            response = _requestFormBAL.CancelRequest(requestId, User.Identity.Name, CloseReason, CloseRemark);
            return Content(JsonConvert.SerializeObject(response));
        }

      
    }
}