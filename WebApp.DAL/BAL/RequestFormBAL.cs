using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.DAL;
using WebApp.DAL.Data;
using WebApp.DAL.Exceptions;
using WebApp.DAL.Helpers;
using WebApp.DAL.Models;
using WebApp.DAL.Models.ResponseModels;


namespace WebApp.DAL.BAL
{
    public class RequestFormBAL
    {
        private readonly CurrencyTypeBAL _currencyTypeBAL = new CurrencyTypeBAL();
        private readonly TaxTypeBAL _taxTypeBAL = new TaxTypeBAL();
        private readonly UserBAL _userBAL = new UserBAL();

        private readonly EmailHelper _emailHelper = new EmailHelper();



        public RequestForm GetFormById(string requestId)
        {
            RequestForm _form = null;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var idToSearch = requestId.ToGuid();
                _form = RequestForm.ToModel(_requestFormDAL.List().Where(w => w.Id.Equals(idToSearch)).FirstOrDefault());
            }
            return _form;
        }

        public ResponseObject<RequestApprovalResponse> Approve(string token, string v, string remarks)
        {
            ResponseObject<RequestApprovalResponse> response = null;
            RequestForm form = null;
            var returnStatus = false;

            bool wasPending = false;
            EmailToken emailToken = new EmailToken
            {
                Lengths = v,
                Token = token
            };
            var tokenParts = emailToken.DecryptEmailToken();
            if (tokenParts != null)
            {
                using (var _requestFormDAL = new RequestFormDAL())
                {
                    form = GetFormByToken(token, v);
                    if (form.Status.Equals("Pending"))
                    {
                        returnStatus = _requestFormDAL.UpdateRequestStatusApproved(tokenParts.ApprovalToken, remarks);
                        wasPending = true;
                    }
                }
                var data = new RequestApprovalResponse
                {
                    DocumentNo = GeneralHelper._getRequestNumber(form.DocumentType, form.PayingEntityCode, form.Month, form.Year, form.Number),
                    //DocumentNo = form.DocumentType + "/"
                    //                + form.PayingEntityCode + "/"
                    //                + string.Format("{0:00}", form.Month) + (new DateTime(Convert.ToInt32(form.Year), 01, 01)).ToString("yy") + "/"
                    //                + string.Format("{0:000000}", form.Number),
                    DocumentType = form.DocumentType
                };
                if (returnStatus)
                {
                    response = new ResponseObject<RequestApprovalResponse>
                    {
                        ResponseType = Constants.SUCCESS,
                        Message = "Request approved",
                        Data = data
                    };
                }
                else if (!wasPending)
                {
                    response = new ResponseObject<RequestApprovalResponse>
                    {
                        ResponseType = Constants.ERROR,
                        Message = "Request already approved",
                        Data = data
                    };
                }
                else
                {
                    response = new ResponseObject<RequestApprovalResponse>
                    {
                        ResponseType = Constants.ERROR,
                        Message = "Failed to approved the request. Something went wrong",
                        Data = data
                    };
                }
            }
            else
            {
                response = new ResponseObject<RequestApprovalResponse>
                {
                    ResponseType = Constants.ERROR,
                    Message = "Invalid request"
                };
            }
            return response;
        }

        public ResponseObject<RequestRejectResponse> Reject(string token, string v, string reason, string remark)
        {
            ResponseObject<RequestRejectResponse> response = null;
            RequestForm form = null;
            var returnStatus = false;
            bool wasPending = false;
            EmailToken emailToken = new EmailToken
            {
                Lengths = v,
                Token = token
            };
            var tokenParts = emailToken.DecryptEmailToken();
            if (tokenParts != null)
            {
                using (var _requestFormDAL = new RequestFormDAL())
                {
                    form = GetFormByToken(token, v);
                    if (form.Status.Equals("Pending"))
                    {
                        returnStatus = _requestFormDAL.UpdateRequestStatusRejected(tokenParts.ApprovalToken, reason, remark);
                        wasPending = true;
                    }
                }
                var data = new RequestRejectResponse
                {
                    DocumentNo = GeneralHelper._getRequestNumber(form.DocumentType, form.PayingEntityCode, form.Month, form.Year, form.Number),
                    //DocumentNo = form.DocumentType + "/"
                    //                + form.PayingEntityCode + "/"
                    //                + string.Format("{0:00}", form.Month) + (new DateTime(Convert.ToInt32(form.Year), 01, 01)).ToString("yy") + "/"
                    //                + string.Format("{0:000000}", form.Number),
                    DocumentType = form.DocumentType,
                    OriginatorName = form.OriginatorName
                };
                if (returnStatus)
                {
                    response = new ResponseObject<RequestRejectResponse>
                    {
                        ResponseType = Constants.SUCCESS,
                        Message = "Request rejected successfully",
                        Data = data
                    };
                }
                else if (!wasPending)
                {
                    response = new ResponseObject<RequestRejectResponse>
                    {
                        ResponseType = Constants.ERROR,
                        Message = "Request already rejected",
                        Data = data
                    };
                }
                else
                {
                    response = new ResponseObject<RequestRejectResponse>
                    {
                        ResponseType = Constants.ERROR,
                        Message = "Failed to reject the request. Something went wrong",
                        Data = data
                    };
                }
            }
            else
            {
                response = new ResponseObject<RequestRejectResponse>
                {
                    ResponseType = Constants.ERROR,
                    Message = "Invalid request"
                };
            }
            return response;
        }




        public bool ApproveRequest(string requestId, string userEmail)
        {
            var returnStatus = false;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var formIdToSearch = requestId.ToGuid();
                var form = _requestFormDAL.List().Where(w => w.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase) && w.Id.Equals(formIdToSearch) && !w.Approvers.Any(a => a.ApprovalStatus == ApprovalStatus.REJECTED)).FirstOrDefault();
                if (form != null)
                {
                    var pendingApproval = form.Approvers.SkipWhile(sw => sw.ApprovalStatus.Equals(ApprovalStatus.APPROVED)).FirstOrDefault();
                    if (pendingApproval != null && pendingApproval.Approver.AppUser.Email.Equals(userEmail))
                    {
                        returnStatus = _requestFormDAL.UpdateRequestStatusApproved(pendingApproval.ApprovalToken, string.Empty);
                    }
                }
            }
            return returnStatus;
        }
        public bool RejectRequest(string requestId, string reason, string userEmail, string remark)
        {
            var returnStatus = false;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var formIdToSearch = requestId.ToGuid();
                var form = _requestFormDAL.List().Where(w => w.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase) && w.Id.Equals(formIdToSearch) && !w.Approvers.Any(a => a.ApprovalStatus == ApprovalStatus.REJECTED)).FirstOrDefault();
                if (form != null)
                {
                    var pendingApproval = form.Approvers.SkipWhile(sw => sw.ApprovalStatus.Equals(ApprovalStatus.APPROVED)).FirstOrDefault();
                    if (pendingApproval != null && pendingApproval.Approver.AppUser.Email.Equals(userEmail))
                    {
                        _requestFormDAL.UpdateRequestStatusRejected(pendingApproval.ApprovalToken, reason, remark);
                        returnStatus = true;
                    }
                }
            }
            return returnStatus;
        }

        public RequestFormDetails GetApproverFormByFormId(string requestId, string email)
        {
            RequestFormDetails form = null;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var id = requestId.ToGuid();
                var form1 = _requestFormDAL.List().Where(w => w.Id.Equals(id)).FirstOrDefault();
                var currentApprover = form1.Approvers.OrderBy(o => o.SequenceNo).Where(w => w.ApprovalStatus.Equals(ApprovalStatus.PENDING)).FirstOrDefault();
                if (currentApprover.Approver.AppUser.Email.Equals(email))
                {
                    form = RequestFormDetails.ToModel(form1);
                }
                //form = RequestFormDetails.ToModel(_requestFormDAL.List().Where(w => w.Id.Equals(id) && w.Originator.Email.Equals(originatorEmail)).FirstOrDefault());
            }
            return form;
        }



        public async Task<ResponseObject<UpdateClarificationResponse>> UpdateClarification(GiveClarificationModel model, string name)
        {
            var responseObject = new ResponseObject<UpdateClarificationResponse>();
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var formIdToSearch = model.RequestId.ToGuid();
                var form = _requestFormDAL.List().Where(w => w.Id.Equals(formIdToSearch)).FirstOrDefault();
                if (form != null)
                {
                    if (form.Originator.Email.Equals(name))
                    {
                        var result = _requestFormDAL.UpdateRequestClarifications(form.Id, model.Clarifications);
                        if (result)
                        {
                            responseObject.ResponseType = Constants.SUCCESS;
                            responseObject.Message = "Clarifications submitted successfully";

                            var details = RequestFormDetails.ToModel(form);
                            var requestedBy = details.Approvals.Where(w => w.ApprovalStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase)).OrderBy(o => o.SequenceNumber).FirstOrDefault();
                            if (requestedBy != null)
                            {
                                EmailToken emailToken = new EmailToken();
                                var isEmailSent = await _emailHelper.SendClarificationGivenEmail(details, new ApprovalRequestEmailMessage
                                {
                                    To = new List<string> { requestedBy.ApproverEmail },
                                    //CC = form.Originator.Email,
                                    SecurityToken = emailToken.GenerateToken(new TokenParts
                                    {
                                        ApprovalToken = requestedBy.ApproverToken,
                                        Reason = "approve_reject_request",
                                        UserEmail = requestedBy.ApproverEmail
                                    })
                                });
                            }
                        }
                        else
                        {
                            responseObject.ResponseType = Constants.ERROR;
                            responseObject.Message = "Something went wrong while updating the clarifications";
                        }
                    }
                    else
                    {
                        responseObject.ResponseType = Constants.ERROR;
                        responseObject.Message = "Cannot edit clarifications for this request";
                    }
                }
                else
                {
                    responseObject.ResponseType = Constants.ERROR;
                    responseObject.Message = "No record found for the current request";
                }
            }
            return responseObject;
        }

        public async Task<bool> SendPPRFRejectedEmail(RequestForm requestForm, string remark)
        {
            var isEmailSent = false;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var idToSearch = requestForm.Id.ToGuid();
                var form = _requestFormDAL.List().Where(w => w.Id.Equals(idToSearch)).FirstOrDefault();
                if (form.Approvers.Any(a => a.ApprovalStatus == ApprovalStatus.REJECTED))
                {
                    isEmailSent = await _emailHelper.SendPPRFRejectedEmail(RequestFormDetails.ToModel(form), remark);
                }
            }
            return isEmailSent;
        }

        public async Task SendPPRFApprovedEmail(RequestForm requestForm)
        {
            var isEmailSent = false;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var idToSearch = requestForm.Id.ToGuid();
                var form = _requestFormDAL.List().Where(w => w.Id.Equals(idToSearch)).FirstOrDefault();
                if (form.Approvers.All(a => a.ApprovalStatus == ApprovalStatus.APPROVED))
                {
                    isEmailSent = await _emailHelper.SendAppovalSuccessfulEmail(RequestFormDetails.ToModel(form));
                }
            }
        }

        public async Task<ResponseObject<SendEmailResponse>> SendApproverEmail(string requestId, string name, string Remark, string urjent)
        {
            var responseObject = new ResponseObject<SendEmailResponse>();
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var formIdToSearch = requestId.ToGuid();
                var form = _requestFormDAL.List().Where(w => w.Id.Equals(formIdToSearch)).FirstOrDefault();
                if (form != null)
                {
                    var approver = form.Approvers.OrderBy(o => o.SequenceNo).Where(w => w.ApprovalStatus.Equals(ApprovalStatus.PENDING)).FirstOrDefault();

                    if (approver != null)
                    {
                        EmailToken emailToken = new EmailToken();
                        var SecurityToken = emailToken.GenerateToken(new TokenParts
                        {
                            ApprovalToken = approver.ApprovalToken.ToString(),
                            Reason = "approve_reject_request",
                            UserEmail = approver.Approver.AppUser.Email
                        });
                        string strApproval = "APPROVAL NEEDED - ";
                        if (!string.IsNullOrEmpty(Remark))
                        {
                            if (urjent == "True")
                            {
                                strApproval = "URJENT   "+strApproval + Remark;
                               _requestFormDAL.UrjentRequestUpdate(requestId.ToGuid(), Remark, urjent);
                            }
                            else
                            {
                                strApproval = strApproval + Remark;
                                _requestFormDAL.UrjentRequestUpdate(requestId.ToGuid(), Remark, urjent);
                            }
                        }


                        var isEmailSent = await _emailHelper.SendApprovalRequestEmail(new ApprovalRequestEmailMessage
                        {
                            To = new List<string> { approver.Approver.AppUser.Email },
                            SecurityToken = SecurityToken,
                            BCC = new List<string> { ConfigurationManager.AppSettings["BCC"] == null ? string.Empty : ConfigurationManager.AppSettings["BCC"] },
                            Subject = strApproval+" " + form.DocumentType + " Request: " + form.PPRFNo + " is awaiting your approval"
                            //CC = form.Originator.Email
                        }, RequestFormDetails.ToModel(form));

                        if (isEmailSent)
                        {
                            responseObject.ResponseType = Constants.SUCCESS;
                            responseObject.Message = "Email sent to the next approver";
                        }
                        else
                        {
                            responseObject.ResponseType = Constants.ERROR;
                            responseObject.Message = "Something went wrong while sending email";
                        }
                    }
                    else
                    {
                        //TODO: send email to originator
                        responseObject.ResponseType = Constants.ERROR;
                        responseObject.Message = "No approver to send email to.";
                    }
                }
                else
                {
                    responseObject.ResponseType = Constants.ERROR;
                    responseObject.Message = "No record found for the current request";
                }
            }
            return responseObject;
        }

        public FileDownloadModel GetFileForRequestId(string requestId)
        {
            FileDownloadModel returnObject = null;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var guidId = requestId.ToGuid();
                var request = _requestFormDAL.List().Where(w => w.Id.Equals(guidId)).FirstOrDefault();
                if (request != null)
                {
                    if (!string.IsNullOrEmpty(request.DocumentSavedName))
                    {
                        //var filename = Path.GetFileName(request.DocumentPath);
                        var path = ConfigurationManager.AppSettings["DocumentsPath"] + Path.DirectorySeparatorChar + requestId.ToString() + Path.GetExtension(request.DocumentSavedName);
                        try
                        {
                            var bytes = File.ReadAllBytes(path);
                            returnObject = new FileDownloadModel
                            {
                                FileBytes = bytes,
                                FileName = request.DocumentName
                            };
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
            }
            return returnObject;
        }

        public FileDownloadModel GetFileByDocID(string docID)
        {
            FileDownloadModel returnObject = null;

            using (var _paymentRequestDocDAL = new PaymentRequestDocsDAL())
            {
                var guidId = docID.ToGuid();
                var doc = _paymentRequestDocDAL.List().Where(w => w.Id.Equals(guidId)).FirstOrDefault();
                if (doc != null)
                {
                    if (!string.IsNullOrEmpty(doc.DocumentSavedName))
                    {
                        //var filename = Path.GetFileName(request.DocumentPath);
                        var path = ConfigurationManager.AppSettings["DocumentsPath"] + Path.DirectorySeparatorChar + doc.DocumentSavedName;
                        try
                        {
                            var bytes = File.ReadAllBytes(path);
                            returnObject = new FileDownloadModel
                            {
                                FileBytes = bytes,
                                FileName = doc.DocumentName
                            };
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
            }
            return returnObject;
        }

        public RequestForm GetFormByToken(string token, string v)
        {
            RequestForm form = null;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                EmailToken emailToken = new EmailToken
                {
                    Lengths = v,
                    Token = token
                };
                var tokenParts = emailToken.DecryptEmailToken();
                form = RequestForm.ToModel(_requestFormDAL.List().Where(w => w.Approvers.Any(a => tokenParts.ApprovalToken.Equals(a.ApprovalToken, StringComparison.OrdinalIgnoreCase))).FirstOrDefault());
            }
            return form;
        }

        public async Task<bool> AskClarificationFor(string requestId, string email)
        {
            var returnVal = false;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var formIdToSearch = requestId.ToGuid();
                var form = _requestFormDAL.List().Where(w => w.Id.Equals(formIdToSearch)).FirstOrDefault();
                if (form != null)
                {
                    returnVal = _requestFormDAL.UpdateClarificationRequired(form.Id, true);
                    if (returnVal)
                    {
                        if (form.Approvers.Where(w => w.ApprovalStatus == ApprovalStatus.PENDING).Count() > 0)
                        {
                            var request = RequestFormDetails.ToModel(form);
                            var requestedBy = request.Approvals.OrderBy(o => o.SequenceNumber).Where(w => w.ApproverEmail.Equals(email, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                            if (requestedBy != null)
                            {
                                await _emailHelper.SendClarificationRequestEmail(request, requestedBy);
                            }
                        }
                    }
                }
            }
            return returnVal;
        }

        public async Task<ResponseObject<RequestClarificationResponse>> AskClarificationBy(string token, string v)
        {
            ResponseObject<RequestClarificationResponse> response = null;
            var returnObject = false;
            var tokenParts = new EmailToken { Lengths = v, Token = token }.DecryptEmailToken();
            var form = GetFormByToken(token, v);
            if (form != null)
            {
                returnObject = await AskClarificationFor(form.Id, tokenParts.UserEmail);
                if (returnObject)
                {
                    var data = new RequestClarificationResponse
                    {
                        DocumentNo = GeneralHelper._getRequestNumber(form.DocumentType, form.PayingEntityCode, form.Month, form.Year, form.Number),
                        //DocumentNo = form.DocumentType + "/"
                        //                + form.PayingEntityCode + "/"
                        //                + string.Format("{0:00}", form.Month) + (new DateTime(Convert.ToInt32(form.Year), 01, 01)).ToString("yy") + "/"
                        //                + string.Format("{0:000000}", form.Number),
                        DocumentType = form.DocumentType
                    };
                    response = new ResponseObject<RequestClarificationResponse>
                    {
                        ResponseType = Constants.SUCCESS,
                        Message = "Request for clarification posted successfully",
                        Data = data
                    };
                }
                else
                {
                    response = new ResponseObject<RequestClarificationResponse>
                    {
                        ResponseType = Constants.ERROR,
                        Message = "Something went wrong while processing your request"
                    };
                }
            }
            else
            {
                response = new ResponseObject<RequestClarificationResponse>
                {
                    ResponseType = Constants.ERROR,
                    Message = "Request does not exist"
                };
            }
            return response;
        }

        public ResponseObject<RequestFormApprovalStatusDetails> GetApprovalByToken(string token, string v)
        {
            EmailToken emailToken = new EmailToken { Token = token, Lengths = v };
            TokenParts parts = emailToken.DecryptEmailToken();
            ResponseObject<RequestFormApprovalStatusDetails> response = null;
            //RequestFormApprovalStatusDetails approval = null;
            if (parts != null)
            {
                using (var _requestFormDAL = new RequestFormDAL())
                {
                    var form = _requestFormDAL.List().Where(w => w.Approvers.Any(a => a.ApprovalToken.Equals(parts.ApprovalToken, StringComparison.OrdinalIgnoreCase))).FirstOrDefault();

                    if (form != null)
                    {
                        response = new ResponseObject<RequestFormApprovalStatusDetails>
                        {
                            ResponseType = "success",
                            Message = "Request found",
                            Data = form.Approvers.Where(w => w.ApprovalToken.Equals(parts.ApprovalToken, StringComparison.OrdinalIgnoreCase)).Select(s => new RequestFormApprovalStatusDetails
                            {
                                ApprovalStatus = s.ApprovalStatus.Equals(ApprovalStatus.APPROVED) ? "Approved" : s.ApprovalStatus.Equals(ApprovalStatus.PENDING) ? "Pending" : "Rejected",
                                ApproverEmail = s.Approver.AppUser.Email,
                                ApproverName = s.Approver.AppUser.Name,
                                ApproverDesignation = s.Approver.AppUser.Designation,
                                ApproverDepartmentName = s.Approver.AppUser.Department != null ? s.Approver.AppUser.Department.Name : string.Empty,
                                QueryCount = s.Queries.Count,
                                HasPendingQuestions = s.Queries.Any(a => a.AnsweredOn == null)
                            }).FirstOrDefault()
                        };
                        //approval = form.Approvers.Where(w => w.ApprovalToken.Equals(parts.ApprovalToken, StringComparison.OrdinalIgnoreCase)).Select(s => new RequestFormApprovalStatusDetails
                        //{
                        //    ApprovalStatus = s.ApprovalStatus.Equals(ApprovalStatus.APPROVED) ? "Approved" : s.ApprovalStatus.Equals(ApprovalStatus.PENDING) ? "Pending" : "Rejected",
                        //    ApproverEmail = s.Approver.AppUser.Email,
                        //    ApproverName = s.Approver.AppUser.Name,
                        //    ApproverDesignation = s.Approver.AppUser.Designation,
                        //    ApproverDepartmentName = s.Approver.AppUser.Department != null ? s.Approver.AppUser.Department.Name : string.Empty,
                        //    QueryCount = s.Queries.Count,
                        //    HasPendingQuestions = s.Queries.Any(a => a.AnsweredOn == null)
                        //}).FirstOrDefault();
                    }
                    else
                    {
                        response = new ResponseObject<RequestFormApprovalStatusDetails>
                        {
                            ResponseType = "error",
                            Message = "Request could not be located."
                        };
                    }
                }
            }
            else
            {
                response = new ResponseObject<RequestFormApprovalStatusDetails>
                {
                    ResponseType = "error",
                    Message = "Invalid request data"
                };
            }
            return response;
            //return approval;
        }

        public async Task<ResponseObject<SaveRequestFormResponse>> SaveForm(Models.SavePaymentProcessingRequestFormViewModel model)
        {
            //var validExtensions = new string[] { ".doc", ".docx", ".xls", ".xlsx", ".pdf" };
            ResponseObject<SaveRequestFormResponse> responseObject = null;
            var returnId = Guid.Empty;
            var isSuccess = false;
            var isAllGood = true;

            if (model != null)
            {
                try
                {
                    var form = GetEntityFromModel(model);
                    if (model.FileAttachmentsDetails != null)  //temporary no control of number of files
                    {
                        var maxFileSize = Convert.ToInt32(ConfigurationManager.AppSettings["maxFileSize"].ToString());
                        var totalFileSize = 0;

                        for (int i = 0; i < model.Files.Count; i++)
                        {
                            totalFileSize += model.Files[i].ContentLength / (1024 * 1024);
                        }

                        if (totalFileSize > maxFileSize)
                        {
                            responseObject = new ResponseObject<SaveRequestFormResponse>
                            {
                                ResponseType = Constants.ERROR,
                                Message = "The total files' size allowed is " + maxFileSize.ToString() + " mb"
                            };

                            isAllGood = false;
                        }
                    }

                    //          if (model.Document != null)
                    //          {
                    //              var extension = Path.GetExtension(model.Document.FileName);
                    //              var fileSize = model.Document.ContentLength / (1024 * 1024);
                    //              var maxFileSize = Convert.ToInt32(ConfigurationManager.AppSettings["maxFileSize"].ToString());
                    //              if (!validExtensions.Contains(extension))
                    //              {
                    //                  responseObject = new ResponseObject<SaveRequestFormResponse>
                    //                  {
                    //                      ResponseType = Constants.ERROR,
                    //                      Message = "unable to upload the file with extension " + extension + ". The valid extension types are " + string.Join(",", validExtensions)
                    //                  };
                    //                  isAllGood = false;
                    //              }
                    //              else if (model.Document.ContentLength > maxFileSize)
                    //              {
                    //                  responseObject = new ResponseObject<SaveRequestFormResponse>
                    //                  {
                    //                      ResponseType = Constants.ERROR,
                    //                      Message = "the maximum allowed filesize is 10 mb"
                    //                  };
                    //                  isAllGood = false;
                    //              }
                    //          }

                    if (isAllGood)
                    {
                        using (var _requestFormDAL = new RequestFormDAL())
                        {
                            returnId = _requestFormDAL.Save(form);
                            isSuccess = !returnId.Equals(Guid.Empty);
                        }
                        if (isSuccess)
                        {
                            if (model.Files != null)
                            {
                                int index = 0;
                                foreach (var doc in model.Files)
                                {
                                    index++;
                                    UploadDocument_Multiple(doc, returnId, index);
                                }
                            }

                            //UploadDocument(model, returnId);

                            using (var _requestFormDAL = new RequestFormDAL())
                            {
                                var request = RequestFormDetails.ToModel(_requestFormDAL.List().Where(w => w.Id.Equals(returnId)).FirstOrDefault());

                                var approver = request.Approvals.OrderBy(o => o.ApproverType).FirstOrDefault();
                                EmailToken emailToken = new EmailToken();
                                var isEmailSent = await _emailHelper.SendApprovalRequestEmail(new ApprovalRequestEmailMessage
                                {
                                    To = new List<string> { approver.ApproverEmail },
                                    BCC = new List<string> { ConfigurationManager.AppSettings["BCC"] == null ? string.Empty : ConfigurationManager.AppSettings["BCC"] },
                                    SecurityToken = emailToken.GenerateToken(new TokenParts
                                    {
                                        ApprovalToken = approver.ApproverToken,
                                        Reason = "approve_reject_request",
                                        UserEmail = approver.ApproverEmail
                                    }),
                                    Subject = request.DocumentType + " APPROVAL NEEDED- Request: " + request.PprfNo + " is awaiting your approval"
                                }, request);
                                if (isEmailSent)
                                {
                                    await SendOriginatorNotification(new RequestForm
                                    {
                                        Id = returnId.ToString()
                                    });
                                    //isEmailSent = _emailHelper.SendNotificationToOriginator(new EmailMessage
                                    //{
                                    //    EmailTo = request.Originator.OriginatorEmail,
                                    //}, request);
                                    responseObject = new ResponseObject<SaveRequestFormResponse>
                                    {
                                        ResponseType = Constants.SUCCESS,
                                        Message = "PPRF created successfully and email dispatched to the approver."
                                    };
                                }
                                else
                                {
                                    responseObject = new ResponseObject<SaveRequestFormResponse>
                                    {
                                        ResponseType = Constants.SUCCESS,
                                        Message = "PPRF created successfully but something went wrong while sending the email to the approver."
                                    };
                                }
                            }
                        }
                    }
                }
                catch (UnableToParseSaveRequestException)
                {
                    responseObject = new ResponseObject<SaveRequestFormResponse>
                    {
                        ResponseType = Constants.ERROR,
                        Message = "Unable to process the request"
                    };
                }
                catch (Exception ex)
                {
                    responseObject = new ResponseObject<SaveRequestFormResponse>
                    {
                        ResponseType = Constants.ERROR,
                        Message = "Something went wrong while creating the PPRF. " + ex.Message
                    };
                }
            }
            return responseObject;
        }
        public  ResponseObject<SaveRequestFormResponse> SaveDraftForm(Models.SavePaymentProcessingRequestFormViewModel model)
        {
           
            ResponseObject<SaveRequestFormResponse> responseObject = null;
            var returnId = Guid.Empty;
            var isSuccess = false;
            var isAllGood = true;

            if (model != null)
            {
                try
                {
                    var form = GetEntityFromModel(model);
                    if (model.FileAttachmentsDetails != null)  //temporary no control of number of files
                    {
                        var maxFileSize = Convert.ToInt32(ConfigurationManager.AppSettings["maxFileSize"].ToString());
                        var totalFileSize = 0;

                        for (int i = 0; i < model.Files.Count; i++)
                        {
                            totalFileSize += model.Files[i].ContentLength / (1024 * 1024);
                        }

                        if (totalFileSize > maxFileSize)
                        {
                            responseObject = new ResponseObject<SaveRequestFormResponse>
                            {
                                ResponseType = Constants.ERROR,
                                Message = "The total files' size allowed is " + maxFileSize.ToString() + " mb"
                            };

                            isAllGood = false;
                        }
                    }

                    if (isAllGood)
                    {
                        using (var _requestFormDAL = new RequestFormDAL())
                        {
                            returnId = _requestFormDAL.SaveDraft(form);
                            isSuccess = !returnId.Equals(Guid.Empty);
                        }
                        if (isSuccess)
                        {
                            if (model.Files != null)
                            {
                                int index = 0;
                                foreach (var doc in model.Files)
                                {
                                    index++;
                                    UploadDocument_Multiple(doc, returnId, index);
                                }
                            }
                            responseObject = new ResponseObject<SaveRequestFormResponse>
                            {
                                ResponseType = Constants.SUCCESS,
                                Message = "PPRF Saved in Draft successfully !."
                            };
                        }
                    }
                }
                catch (UnableToParseSaveRequestException)
                {
                    responseObject = new ResponseObject<SaveRequestFormResponse>
                    {
                        ResponseType = Constants.ERROR,
                        Message = "Unable to process the request"
                    };
                }
                catch (Exception ex)
                {
                    responseObject = new ResponseObject<SaveRequestFormResponse>
                    {
                        ResponseType = Constants.ERROR,
                        Message = "Something went wrong while creating the PPRF. " + ex.Message
                    };
                }
            }
            return responseObject;
        }
        public async Task<ResponseObject<SaveRequestFormResponse>> UpdateDraftForm(Models.SavePaymentProcessingRequestFormViewModel model)
        {

            ResponseObject<SaveRequestFormResponse> responseObject = null;
            var returnId = Guid.Empty;
            var isSuccess = false;
            var isAllGood = true;

            if (model != null)
            {
                try
                {
                   
                    var id = model.RequestId.ToGuid();
                    var form = UpdateEntityFromModel(model);
                    if (model.FileAttachmentsDetails != null)  //temporary no control of number of files
                    {
                        var maxFileSize = Convert.ToInt32(ConfigurationManager.AppSettings["maxFileSize"].ToString());
                        var totalFileSize = 0;

                        for (int i = 0; i < model.Files.Count; i++)
                        {
                            totalFileSize += model.Files[i].ContentLength / (1024 * 1024);
                        }

                        if (totalFileSize > maxFileSize)
                        {
                            responseObject = new ResponseObject<SaveRequestFormResponse>
                            {
                                ResponseType = Constants.ERROR,
                                Message = "The total files' size allowed is " + maxFileSize.ToString() + " mb"
                            };

                            isAllGood = false;
                        }
                    }

                    if (isAllGood)
                    {
                        using (var _requestFormDAL = new RequestFormDAL())
                        {
                            returnId = _requestFormDAL.UpdateDraft(id, form);
                            isSuccess = !returnId.Equals(Guid.Empty);
                        }
                        if (isSuccess)
                        {
                            if (model.Files != null)
                            {
                                int index = 0;
                                foreach (var doc in model.Files)
                                {
                                    index++;
                                    UploadDocument_Multiple(doc, returnId, index);
                                }
                            }
                            using (var _requestFormDAL = new RequestFormDAL())
                            {
                                var request = RequestFormDetails.ToModel(_requestFormDAL.List().Where(w => w.Id.Equals(returnId)).FirstOrDefault());

                                var approver = request.Approvals.OrderBy(o => o.ApproverType).FirstOrDefault();
                                EmailToken emailToken = new EmailToken();
                                var isEmailSent = await _emailHelper.SendApprovalRequestEmail(new ApprovalRequestEmailMessage
                                {
                                    To = new List<string> { approver.ApproverEmail },
                                    BCC = new List<string> { ConfigurationManager.AppSettings["BCC"] == null ? string.Empty : ConfigurationManager.AppSettings["BCC"] },
                                    SecurityToken = emailToken.GenerateToken(new TokenParts
                                    {
                                        ApprovalToken = approver.ApproverToken,
                                        Reason = "approve_reject_request",
                                        UserEmail = approver.ApproverEmail
                                    }),
                                    Subject = request.DocumentType + " APPROVAL NEEDED- Request: " + request.PprfNo + " is awaiting your approval"
                                }, request);
                                if (isEmailSent)
                                {
                                    await SendOriginatorNotification(new RequestForm
                                    {
                                        Id = returnId.ToString()
                                    });
                                    //isEmailSent = _emailHelper.SendNotificationToOriginator(new EmailMessage
                                    //{
                                    //    EmailTo = request.Originator.OriginatorEmail,
                                    //}, request);
                                    responseObject = new ResponseObject<SaveRequestFormResponse>
                                    {
                                        ResponseType = Constants.SUCCESS,
                                        Message = "PPRF created successfully and email dispatched to the approver."
                                    };
                                }
                                else
                                {
                                    responseObject = new ResponseObject<SaveRequestFormResponse>
                                    {
                                        ResponseType = Constants.SUCCESS,
                                        Message = "PPRF created successfully but something went wrong while sending the email to the approver."
                                    };
                                }
                            }
                            responseObject = new ResponseObject<SaveRequestFormResponse>
                            {
                                ResponseType = Constants.SUCCESS,
                                Message = "PPRF Saved successfully and email dispatched to the approver !."
                            };
                        }
                    }
                }
                catch (UnableToParseSaveRequestException)
                {
                    responseObject = new ResponseObject<SaveRequestFormResponse>
                    {
                        ResponseType = Constants.ERROR,
                        Message = "Unable to process the request"
                    };
                }
                catch (Exception ex)
                {
                    responseObject = new ResponseObject<SaveRequestFormResponse>
                    {
                        ResponseType = Constants.ERROR,
                        Message = "Something went wrong while creating the PPRF. " + ex.Message
                    };
                }
            }
            return responseObject;
        }
        public RequestFormDetails GetOriginatorFormByFormId(string requestId, string originatorEmail)
        {
            //string[] months = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            RequestFormDetails form = null;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var id = requestId.ToGuid();
                form = RequestFormDetails.ToModel(_requestFormDAL.List().Where(w => w.Id.Equals(id) &&
                                                        (w.Originator.Email.Equals(originatorEmail) || w.CreatedBy.Email.Equals(originatorEmail))).FirstOrDefault());
            }
            return form;
        }

        public RequestFormsViewModel RequestFormsForApprover(string email, RequestFormsViewModel model)
        {
            var forms = new List<RequestForm>();
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var list = _requestFormDAL
                                .List()
                                .Where(w => !w.Approvers.Any(a => a.ApprovalStatus == ApprovalStatus.REJECTED))
                                .Where(w => w.Approvers
                                            .Where(a => a.ApprovalStatus == ApprovalStatus.PENDING)
                                            .OrderBy(o => o.SequenceNo)
                                            .FirstOrDefault()
                                            .Approver.AppUser.Email.Equals(email));

                model.Pager.TotalItems = list.Count();
                var pagedList = list.OrderByDescending(o => o.CreatedOn)
                                    .Skip((model.Pager.CurrentPage - 1) * model.Pager.ItemsPerPage)
                                    .Take(model.Pager.ItemsPerPage).ToList();

                model.RequestForms.AddRange(RequestForm.ToModel(pagedList));
                model.RequestForms.ForEach(f =>
                {
                    f.AskAnotherQuestion = false;
                    var request = list.Where(w => w.Id.ToString().Equals(f.Id, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (request != null)
                    {
                        var approver = request.Approvers.Where(w => w.Approver.AppUser.Email.Equals(email)).FirstOrDefault();
                        if (approver != null)
                        {
                            f.AskAnotherQuestion = approver.Queries.Count < 2;
                            f.IsQuestionAsked = approver.IsQuestionAsked;
                        }
                    }
                });
            }
            return model;
        }

        public OriginatorFormsViewModel RequestFormsForOriginatorOrCreator(string email, OriginatorFormsViewModel model)
        {
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var list = _requestFormDAL.List().Where(w => w.Originator.Email.Equals(email, StringComparison.OrdinalIgnoreCase) || w.CreatedBy.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
                model.Pager.TotalItems = list.Count();
                var pagedList = list.OrderByDescending(o => o.CreatedOn)
                                    .Skip((model.Pager.CurrentPage - 1) * model.Pager.ItemsPerPage)
                                    .Take(model.Pager.ItemsPerPage).ToList();

                model.RequestForms.AddRange(RequestForm.ToModel(pagedList));

                model.RequestForms.ForEach(f =>
                {
                    f.AskAnotherQuestion = false;
                    var request = list.Where(w => w.Id.ToString().Equals(f.Id, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (request != null)
                    {
                        var approver = request.Approvers.Where(w => w.ApprovalStatus == ApprovalStatus.PENDING).OrderBy(o => o.SequenceNo).FirstOrDefault();
                        //var approver = request.Approvers.Where(w => w.Approver.AppUser.Email.Equals(email)).FirstOrDefault();
                        if (approver != null)
                        {
                            f.AskAnotherQuestion = approver.Queries.Count < 2;
                            f.IsQuestionAsked = approver.IsQuestionAsked;
                            if (f.IsQuestionAsked)
                            {
                                f.RequestQuestions = approver.Queries.Where(w => w.AnsweredOn == null).Select(s => new RequestQuestion
                                {
                                    Id = s.Id.ToString(),
                                    Question = s.Question
                                }).ToList();
                            }
                        }
                    }
                });
            }
            return model;
        }

        public OriginatorFormsViewModel RequestFormsForOriginator(string email, OriginatorFormsViewModel model)
        {
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var list = _requestFormDAL.List().Where(w => w.Originator.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
                model.Pager.TotalItems = list.Count();
                var pagedList = list.OrderByDescending(o => o.CreatedOn)
                                    .Skip((model.Pager.CurrentPage - 1) * model.Pager.ItemsPerPage)
                                    .Take(model.Pager.ItemsPerPage).ToList();

                model.RequestForms.AddRange(RequestForm.ToModel(pagedList));

                model.RequestForms.ForEach(f =>
                {
                    f.AskAnotherQuestion = false;
                    var request = list.Where(w => w.Id.ToString().Equals(f.Id, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (request != null)
                    {
                        var approver = request.Approvers.Where(w => w.ApprovalStatus == ApprovalStatus.PENDING).OrderBy(o => o.SequenceNo).FirstOrDefault();
                        //var approver = request.Approvers.Where(w => w.Approver.AppUser.Email.Equals(email)).FirstOrDefault();
                        if (approver != null)
                        {
                            f.AskAnotherQuestion = approver.Queries.Count < 2;
                            f.IsQuestionAsked = approver.IsQuestionAsked;
                            if (f.IsQuestionAsked)
                            {
                                f.RequestQuestions = approver.Queries.Where(w => w.AnsweredOn == null).Select(s => new RequestQuestion
                                {
                                    Id = s.Id.ToString(),
                                    Question = s.Question
                                }).ToList();
                            }
                        }
                    }
                });
            }
            return model;
        }

        public ApprovedFormsViewModel GetApprovedFormsPendingClosure(ApprovedFormsViewModel model)
        {
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var list = _requestFormDAL.List().Where(w => w.Approvers.All(a => a.ApprovalStatus == ApprovalStatus.APPROVED) && !w.IsClosed);
                model.Pager.TotalItems = list.Count();
                var pagedList = list.OrderByDescending(o => o.CreatedOn)
                                    .Skip((model.Pager.CurrentPage - 1) * model.Pager.ItemsPerPage)
                                    .Take(model.Pager.ItemsPerPage).ToList();

                model.RequestForms.AddRange(RequestForm.ToModel(pagedList));
            }
            return model;
        }


        private void UploadDocument(SavePaymentProcessingRequestFormViewModel model, Guid requestId)
        {
            if (model.Document != null)
            {
                var extension = Path.GetExtension(model.Document.FileName);
                var path = ConfigurationManager.AppSettings["DocumentsPath"] + Path.DirectorySeparatorChar + requestId.ToString() + extension;
                //var path = Path.Combine(Constants.PAYING_ENTITY_LOGO_REQUEST_REFERENCE_DOCUMENT, requestId.ToString() + extension);
                model.Document.SaveAs(path);
                using (var _requestFormDAL = new RequestFormDAL())
                {
                    _requestFormDAL.UpdateRequestReferenceDocument(requestId, model.Document.FileName, path);
                }
            }
        }

        private void UploadDocument_Multiple(HttpPostedFileBase document, Guid requestId, int index)
        {
            if (document != null)
            {
                try
                {
                    var extension = Path.GetExtension(document.FileName);
                    var falseName = requestId + "-" + GeneralHelper.CurrentDate().Ticks + "-" + index + extension;
                    var path = ConfigurationManager.AppSettings["DocumentsPath"] + Path.DirectorySeparatorChar + falseName;
                    string directoryPath =  ConfigurationManager.AppSettings["DocumentsPath"] + Path.DirectorySeparatorChar ;

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    document.SaveAs(path);

                    using (var _requestDL = new RequestFormDAL())
                    {
                        _requestDL.AddRequestDocument(new PaymentRequestDocuments
                        {
                            DocumentSavedName = falseName,
                            DocumentName = document.FileName,
                            PaymentRequestFormId = requestId
                        });
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        private PaymentRequestForm GetEntityFromModel(SavePaymentProcessingRequestFormViewModel model)
        {
            try
            {
                var currency = _currencyTypeBAL.GetById(model.CurrencyType.Id.ToGuid());
                var GoodsAndServices = new List<Data.GoodsAndService>();
                var Approvers = new List<RequestApprover>();

                model.GoodsAndServices.ForEach(f =>
                {
                    var PercentageValue = 0m;
                    Models.TaxType taxType = null;
                    if (f.TaxType != null && f.TaxType.Id != null)
                    {
                        taxType = _taxTypeBAL.FindById(f.TaxType.Id.ToGuid());
                        PercentageValue = taxType == null ? 0m : taxType.PercentageValue;
                    }

                    var service = new Data.GoodsAndService
                    {
                        Description = f.Description,
                        Amount = f.Amount,
                        Quantity = f.Quantity,
                        TaxTypeId = taxType == null ? null : (Guid?)taxType.Id.ToGuid(),
                        //AmountUSD = f.Amount * currency.USDValue,
                        //AmountEuro = f.Amount * currency.EuroValue,
                        AmountUSD = f.Amount * model.USDExRate,
                        AmountEuro = f.Amount * model.EuroExRate,
                        TaxAmount = f.Amount * PercentageValue / 100,
                        //TaxAmountUSD = f.Amount * PercentageValue / 100 * currency.USDValue,
                        //TaxAmountEuro = f.Amount * PercentageValue / 100 * currency.EuroValue
                        TaxAmountUSD = f.Amount * PercentageValue / 100 * model.USDExRate,
                        TaxAmountEuro = f.Amount * PercentageValue / 100 * model.EuroExRate
                    };
                    GoodsAndServices.Add(service);
                });
                model.Approvers.ForEach(f =>
                {
                    var approver = new RequestApprover
                    {
                        ApproverId = f.UserGroupId.ToGuid(),
                        ApprovalStatus = ApprovalStatus.PENDING,
                        ApprovalToken = Guid.NewGuid().ToString(),
                        SequenceNo = f.SequenceNo
                    };
                    Approvers.Add(approver);
                });
                var form = new PaymentRequestForm();
                form.USDExRate = model.USDExRate;
                form.EuroExRate = model.EuroExRate;
                form.DocumentType = model.DocumentType;
                form.PayingEntityId = model.PayingEntity.Id.ToGuid();
                form.PprfDate = model.PPRFDate;
                form.DueDate = model.DueDate;
                //form.CountryId = model.Country.Id.ToGuid();
                form.Month = Convert.ToInt32(model.Month);
                form.Year = model.Year;
                form.DepartmentId = model.Department.Id.ToGuid();
                form.DepartmentsAccountId = model.DepartmentsAccount.Id.ToGuid();
                form.FrequencyTypeId = model.FrequencyType.Id.ToGuid();
                form.PaymentMethodId = model.PaymentMethod.Id.ToGuid();
                form.ExpenseTypeId = model.ExpenseType.Id.ToGuid();

                if (model.RestrictedPayeeOnly != 'N')
                {
                    form.PayeeBankAccountDetailId = model.PayeeBank.Id.ToGuid();
                }

                form.Description = model.Description;
                form.Remarks = model.Remarks;
                form.CurrencyId = model.CurrencyType.Id.ToGuid();
                form.GoodsAndServices.AddRange(GoodsAndServices);

                form.Tax2TypeId = model.TaxType2 != null ? model.TaxType2.Id.ToGuid() : (Guid?)null;
                form.Tax2Amount = model.TaxAmount2;

                //form.Tax2AmountUSD = model.TaxAmount2 * currency.USDValue;
                //form.Tax2AmountEuro = model.TaxAmount2 * currency.EuroValue;
                form.Tax2AmountUSD = model.TaxAmount2 * model.USDExRate;
                form.Tax2AmountEuro = model.TaxAmount2 * model.EuroExRate;

                form.Tax3TypeId = model.TaxType3 != null ? model.TaxType3.Id.ToGuid() : (Guid?)null;
                form.Tax3Amount = model.TaxAmount3;

                //form.Tax3AmountUSD = model.TaxAmount3 * currency.USDValue;
                //form.Tax3AmountEuro = model.TaxAmount3 * currency.EuroValue;
                form.Tax3AmountUSD = model.TaxAmount3 * model.USDExRate;
                form.Tax3AmountEuro = model.TaxAmount3 * model.EuroExRate;

                form.OriginatorId = _userBAL.FindByEmail(model.Originator.Email).Id.ToGuid();
                form.Approvers.AddRange(Approvers);
                form.CreatedById = _userBAL.FindByEmail(model.CreatedByEmail).Id.ToGuid();
                form.CreatedOn = GeneralHelper.CurrentDate();

                form.BudgetPPRFNoId = model.BudgetOrder != null ? model.BudgetOrder.BudgetPPRFId.ToGuid() : (Guid?)null;
                form.BudgetValidFrom = model.BudgetValidFrom;
                form.BudgetValidTo = model.BudgetValidTo;

                if (model.RestrictedPayeeOnly == 'Y')
                {
                    form.RestrictedPayeeOnly = true;
                }
                else if (model.RestrictedPayeeOnly == 'N')
                {
                    form.RestrictedPayeeOnly = false;
                }

                form.BudgetApprovedAmt = model.BudgetApprovedAmt;
                form.BudgetApprovedAmtUSD = model.BudgetApprovedAmtUSD;
                form.BudgetApprovedAmtEuro = model.BudgetApprovedAmtEuro;

                form.BudgetSpentAmt = model.BudgetApprovedAmt;
                form.BudgetSpentAmtUSD = model.BudgetSpentAmtUSD;
                form.BudgetSpentAmtEuro = model.BudgetSpentAmtEuro;

                return form;
            }
            catch (Exception)
            {
                throw new UnableToParseSaveRequestException("Unable to parse the save request");
            }
        }

        private PaymentRequestForm UpdateEntityFromModel(SavePaymentProcessingRequestFormViewModel model)
        {
            try
            {
                var currency = _currencyTypeBAL.GetById(model.CurrencyType.Id.ToGuid());
                var GoodsAndServices = new List<Data.GoodsAndService>();
                var Approvers = new List<RequestApprover>();

                model.GoodsAndServices.ForEach(f =>
                {
                    var PercentageValue = 0m;
                    Models.TaxType taxType = null;
                    if (f.TaxType != null && f.TaxType.Id != null)
                    {
                        taxType = _taxTypeBAL.FindById(f.TaxType.Id.ToGuid());
                        PercentageValue = taxType == null ? 0m : taxType.PercentageValue;
                    }

                    var service = new Data.GoodsAndService
                    {
                        Description = f.Description,
                        Amount = f.Amount,
                        Quantity = f.Quantity,
                        TaxTypeId = taxType == null ? null : (Guid?)taxType.Id.ToGuid(),
                        //AmountUSD = f.Amount * currency.USDValue,
                        //AmountEuro = f.Amount * currency.EuroValue,
                        AmountUSD = f.Amount * model.USDExRate,
                        AmountEuro = f.Amount * model.EuroExRate,
                        TaxAmount = f.Amount * PercentageValue / 100,
                        //TaxAmountUSD = f.Amount * PercentageValue / 100 * currency.USDValue,
                        //TaxAmountEuro = f.Amount * PercentageValue / 100 * currency.EuroValue
                        TaxAmountUSD = f.Amount * PercentageValue / 100 * model.USDExRate,
                        TaxAmountEuro = f.Amount * PercentageValue / 100 * model.EuroExRate
                    };
                    GoodsAndServices.Add(service);
                });
                model.Approvers.ForEach(f =>
                {
                    var approver = new RequestApprover
                    {
                        ApproverId = f.UserGroupId.ToGuid(),
                        ApprovalStatus = ApprovalStatus.PENDING,
                        ApprovalToken = Guid.NewGuid().ToString(),
                        SequenceNo = f.SequenceNo
                    };
                    Approvers.Add(approver);
                });
                var form = new PaymentRequestForm();
                form.USDExRate = model.USDExRate;
                form.EuroExRate = model.EuroExRate;
                form.DocumentType = model.DocumentType;
                form.PayingEntityId = model.PayingEntity.Id.ToGuid();
                form.PprfDate = model.PPRFDate;
                form.DueDate = model.DueDate;
                //form.CountryId = model.Country.Id.ToGuid();
                form.Month = Convert.ToInt32(model.Month);
                form.Year = model.Year;
                form.DepartmentId = model.Department.Id.ToGuid();
                form.DepartmentsAccountId = model.DepartmentsAccount.Id.ToGuid();
                form.FrequencyTypeId = model.FrequencyType.Id.ToGuid();
                form.PaymentMethodId = model.PaymentMethod.Id.ToGuid();
                form.ExpenseTypeId = model.ExpenseType.Id.ToGuid();

                if (model.RestrictedPayeeOnly != 'N')
                {
                    form.PayeeBankAccountDetailId = model.PayeeBank.Id.ToGuid();
                }

                form.Description = model.Description;
                form.Remarks = model.Remarks;
                form.CurrencyId = model.CurrencyType.Id.ToGuid();
                form.GoodsAndServices.AddRange(GoodsAndServices);

                form.Tax2TypeId = model.TaxType2 != null ? model.TaxType2.Id.ToGuid() : (Guid?)null;
                form.Tax2Amount = model.TaxAmount2;

                //form.Tax2AmountUSD = model.TaxAmount2 * currency.USDValue;
                //form.Tax2AmountEuro = model.TaxAmount2 * currency.EuroValue;
                form.Tax2AmountUSD = model.TaxAmount2 * model.USDExRate;
                form.Tax2AmountEuro = model.TaxAmount2 * model.EuroExRate;

                form.Tax3TypeId = model.TaxType3 != null ? model.TaxType3.Id.ToGuid() : (Guid?)null;
                form.Tax3Amount = model.TaxAmount3;

                //form.Tax3AmountUSD = model.TaxAmount3 * currency.USDValue;
                //form.Tax3AmountEuro = model.TaxAmount3 * currency.EuroValue;
                form.Tax3AmountUSD = model.TaxAmount3 * model.USDExRate;
                form.Tax3AmountEuro = model.TaxAmount3 * model.EuroExRate;

                form.OriginatorId = _userBAL.FindByEmail(model.Originator.Email).Id.ToGuid();
                form.Approvers.AddRange(Approvers);
                form.CreatedById = _userBAL.FindByEmail(model.CreatedByEmail).Id.ToGuid();
                form.CreatedOn = GeneralHelper.CurrentDate();

                form.BudgetPPRFNoId = model.BudgetOrder != null ? model.BudgetOrder.BudgetPPRFId.ToGuid() : (Guid?)null;
                form.BudgetValidFrom = model.BudgetValidFrom;
                form.BudgetValidTo = model.BudgetValidTo;

                if (model.RestrictedPayeeOnly == 'Y')
                {
                    form.RestrictedPayeeOnly = true;
                }
                else if (model.RestrictedPayeeOnly == 'N')
                {
                    form.RestrictedPayeeOnly = false;
                }

                form.BudgetApprovedAmt = model.BudgetApprovedAmt;
                form.BudgetApprovedAmtUSD = model.BudgetApprovedAmtUSD;
                form.BudgetApprovedAmtEuro = model.BudgetApprovedAmtEuro;

                form.BudgetSpentAmt = model.BudgetApprovedAmt;
                form.BudgetSpentAmtUSD = model.BudgetSpentAmtUSD;
                form.BudgetSpentAmtEuro = model.BudgetSpentAmtEuro;

                return form;
            }
            catch (Exception)
            {
                throw new UnableToParseSaveRequestException("Unable to parse the save request");
            }
        }

        public async Task<bool> SendOriginatorNotification(RequestForm requestForm)
        {
            RequestFormDetails request = null;
            var isSent = false;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var IdToSearch = requestForm.Id.ToGuid();
                request = RequestFormDetails.ToModel(_requestFormDAL.List().Where(w => w.Id.Equals(IdToSearch)).FirstOrDefault());
            }
            if (request != null)
            {
                isSent = await _emailHelper.SendNotificationToOriginator(new EmailMessage
                {
                    To = new List<string> { request.Originator.OriginatorEmail },
                    CC = new List<string> { request.CreatedByEmail }
                }, request);
            }
            return isSent;
        }

        public ResponseObject<CloseRequestResponse> CloseRequest(RequestForm requestForm, UserDetailsModel user, string CloseReason, string CloseRemark)
        {
            ResponseObject<CloseRequestResponse> response = null;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                try
                {
                    var IdToSearch = requestForm.Id.ToGuid();
                    var request = RequestFormDetails.ToModel(_requestFormDAL.List().Where(w => w.Id.Equals(IdToSearch)).FirstOrDefault());
                    if (request != null)
                    {
                        if (request.IsClosed)
                        {
                            response = new ResponseObject<CloseRequestResponse>
                            {
                                ResponseType = Constants.ERROR,
                                Message = "Request already closed"
                            };
                        }
                        else if (request.Status.Equals("pending", StringComparison.OrdinalIgnoreCase))
                        {
                            response = new ResponseObject<CloseRequestResponse>
                            {
                                ResponseType = Constants.ERROR,
                                Message = "Request still under processing"
                            };
                        }
                        else if (request.Status.Equals("rejected", StringComparison.OrdinalIgnoreCase))
                        {
                            response = new ResponseObject<CloseRequestResponse>
                            {
                                ResponseType = Constants.ERROR,
                                Message = "The request is rejected. It cannot be closed"
                            };
                        }
                        else if (request.Status.Equals("approved", StringComparison.OrdinalIgnoreCase))
                        {
                            var result = _requestFormDAL.CloseRequest(IdToSearch, user.Id,  CloseReason,  CloseRemark);
                            if (result)
                            {
                                response = new ResponseObject<CloseRequestResponse>
                                {
                                    ResponseType = Constants.SUCCESS,
                                    Message = "The request is now closed"
                                };
                            }
                            else
                            {
                                response = new ResponseObject<CloseRequestResponse>
                                {
                                    ResponseType = Constants.ERROR,
                                    Message = "Unable to close the request. Something went wrong."
                                };
                            }
                        }
                    }
                    else
                    {
                        response = new ResponseObject<CloseRequestResponse>
                        {
                            ResponseType = Constants.ERROR,
                            Message = "Request not found"
                        };
                    }
                }
                catch (Exception)
                {
                    response = new ResponseObject<CloseRequestResponse>
                    {
                        ResponseType = Constants.ERROR,
                        Message = "Something went wrong while processing your request."
                    };
                }
            }
            return response;
        }
        public ResponseObject<NotesResponse> NotesUpdate(RequestForm requestForm, UserDetailsModel user, string Notes)
        {
            ResponseObject<NotesResponse> response = null;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                try
                {
                    var IdToSearch = requestForm.Id.ToGuid();
                    var result = _requestFormDAL.NotesUpdate(IdToSearch, user.Id, Notes);

                }
                catch (Exception)
                {
                    response = new ResponseObject<NotesResponse>
                    {
                        ResponseType = Constants.ERROR,
                        Message = "Something went wrong while processing your request."
                    };
                }
            }
            return response;
        }
        public RequestFormDetails GetApprovedRequest(string requestId)
        {
            RequestFormDetails form = null;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var id = requestId.ToGuid();
                var form1 = _requestFormDAL.List().Where(w => w.Id.Equals(id) && w.Approvers.All(a => a.ApprovalStatus == ApprovalStatus.APPROVED)).FirstOrDefault();
                if (form1 != null)
                {
                    form = RequestFormDetails.ToModel(form1);
                }
            }
            return form;
        }



        public ClosedRequestsViewModel GetClosedRequests(ClosedRequestsViewModel model)
        {
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var list = _requestFormDAL.List().Where(w => w.Approvers.All(a => a.ApprovalStatus == ApprovalStatus.APPROVED) && w.IsClosed);
                if (model.Filters != null)
                {
                    if (model.Filters.StartDate != null)
                        list = list.Where(w => w.PprfDate >= model.Filters.StartDate);
                    if (model.Filters.EndDate != null)
                        list = list.Where(w => w.PprfDate <= model.Filters.EndDate);
                    //if (!string.IsNullOrEmpty(model.SearchCriteria.PPRFNumber))
                    //list = list.Where(w=>w.)
                }

                if (model.Pager == null)
                {
                    model.RequestForms.AddRange(RequestForm.ToModel(list.OrderBy(o => o.CreatedOn).ToList()));
                }
                else
                {
                    model.Pager.TotalItems = list.Count();
                    var pagedList = list.OrderByDescending(o => o.CreatedOn)
                                    .Skip((model.Pager.CurrentPage - 1) * model.Pager.ItemsPerPage)
                                    .Take(model.Pager.ItemsPerPage).ToList();
                    model.RequestForms.AddRange(RequestForm.ToModel(pagedList));
                }
            }
            return model;
        }


        public ClosedRequestsViewModel GetClosedOrRejectedRequests(ClosedRequestsViewModel model)
        {
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var list = _requestFormDAL.List().Where(w => w.Approvers.Any(a => a.ApprovalStatus == ApprovalStatus.REJECTED) || w.IsClosed);
                if (model.Sort != null)
                {
                    if (model.Sort.SortParameter.Equals("PprfDate", StringComparison.OrdinalIgnoreCase))
                        if (model.Sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                            list = list.OrderByDescending(o => o.PprfDate);
                        else list = list.OrderBy(o => o.PprfDate);
                    if (model.Sort.SortParameter.Equals("DueDate", StringComparison.OrdinalIgnoreCase))
                        if (model.Sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                            list = list.OrderByDescending(o => o.DueDate);
                        else list = list.OrderBy(o => o.DueDate);
                    if (model.Sort.SortParameter.Equals("Originator", StringComparison.OrdinalIgnoreCase))
                        if (model.Sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                            list = list.OrderByDescending(o => o.Originator.Name);
                        else list = list.OrderBy(o => o.Originator.Name);
                    if (model.Sort.SortParameter.Equals("PayingEntity", StringComparison.OrdinalIgnoreCase))
                        if (model.Sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                            list = list.OrderByDescending(o => o.PayingEntity.Name);
                        else list = list.OrderBy(o => o.PayingEntity.Name);
                    if (model.Sort.SortParameter.Equals("Description", StringComparison.OrdinalIgnoreCase))
                        if (model.Sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                            list = list.OrderByDescending(o => o.Description);
                        else list = list.OrderBy(o => o.Description);

                    if (model.Sort.SortParameter.Equals("Department", StringComparison.OrdinalIgnoreCase))
                        if (model.Sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                            list = list.OrderByDescending(o => o.Department.Name);
                        else list = list.OrderBy(o => o.Department.Name);

                    if (model.Sort.SortParameter.Equals("TotalValue", StringComparison.OrdinalIgnoreCase))
                        if (model.Sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                            list = list.OrderByDescending(o => o.GoodsAndServices.Sum(s => s.Amount + s.TaxAmount));
                        else list = list.OrderBy(o => o.GoodsAndServices.Sum(s => s.Amount + s.TaxAmount));

                    if (model.Sort.SortParameter.Equals("PprfNo", StringComparison.OrdinalIgnoreCase))
                        if (model.Sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                            list = list.OrderByDescending(o => o.PPRFNo)
                                        .ThenByDescending(t => t.PayingEntity.Name)
                                        .ThenByDescending(t => t.Year)
                                        .ThenByDescending(t => t.Month)
                                        .ThenByDescending(t => t.AutoGeneratedSequence);
                        else list = list.OrderBy(o => o.DocumentType)
                                        .ThenBy(t => t.PayingEntity.Name)
                                        .ThenBy(t => t.Year)
                                        .ThenBy(t => t.Month)
                                        .ThenBy(t => t.AutoGeneratedSequence);
                    if (model.Sort.SortParameter.Equals("Status", StringComparison.OrdinalIgnoreCase))
                        if (model.Sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                            list = list.OrderByDescending(o => o.Status);
                        else list = list.OrderBy(o => o.Status);
                }
                if (model.Filters != null)
                {
                    if (model.Filters.StartDate != null)
                        list = list.Where(w => w.PprfDate >= model.Filters.StartDate);
                    if (model.Filters.EndDate != null)
                        list = list.Where(w => w.PprfDate <= model.Filters.EndDate);
                    if (!string.IsNullOrEmpty(model.Filters.Status))
                    {
                        if (model.Filters.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                            list = list.Where(w => w.IsCancelled);
                        else if (model.Filters.Status.Equals("Closed", StringComparison.OrdinalIgnoreCase))
                            list = list.Where(w => w.IsClosed);
                        else
                            list = list.Where(w => w.Status.ToLower() == model.Filters.Status.ToLower());
                    }
                }

                if (model.Pager == null)
                {
                    model.RequestForms.AddRange(RequestForm.ToModel(list.OrderBy(o => o.CreatedOn).ToList()));
                }
                else
                {
                    model.Pager.TotalItems = list.Count();
                    var pagedList = list//.OrderByDescending(o => o.CreatedOn)
                                    .Skip((model.Pager.CurrentPage - 1) * model.Pager.ItemsPerPage)
                                    .Take(model.Pager.ItemsPerPage).ToList();
                    model.RequestForms.AddRange(RequestForm.ToModel(pagedList));
                }
            }
            return model;
        }

        public ResponseObject<RequestFormsViewModel> GetAllRequests(RequestFormsViewModel model)
        {
            ResponseObject<RequestFormsViewModel> response = null;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var list = _requestFormDAL.List();
                if (model.Sort != null)
                {
                    if (model.Sort.SortParameter.Equals("PprfDate", StringComparison.OrdinalIgnoreCase))
                        if (model.Sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                            list = list.OrderByDescending(o => o.PprfDate);
                        else list = list.OrderBy(o => o.PprfDate);
                    if (model.Sort.SortParameter.Equals("DueDate", StringComparison.OrdinalIgnoreCase))
                        if (model.Sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                            list = list.OrderByDescending(o => o.DueDate);
                        else list = list.OrderBy(o => o.DueDate);
                    if (model.Sort.SortParameter.Equals("Originator", StringComparison.OrdinalIgnoreCase))
                        if (model.Sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                            list = list.OrderByDescending(o => o.Originator.Name);
                        else list = list.OrderBy(o => o.Originator.Name);
                    if (model.Sort.SortParameter.Equals("PayingEntity", StringComparison.OrdinalIgnoreCase))
                        if (model.Sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                            list = list.OrderByDescending(o => o.PayingEntity.Name);
                        else list = list.OrderBy(o => o.PayingEntity.Name);
                    if (model.Sort.SortParameter.Equals("Payee", StringComparison.OrdinalIgnoreCase))
                        if (model.Sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                            list = list.OrderByDescending(o => o.PayeeBankAccountDetail.Payee.Name);
                        else list = list.OrderBy(o => o.PayeeBankAccountDetail.Payee.Name);
                    if (model.Sort.SortParameter.Equals("Description", StringComparison.OrdinalIgnoreCase))
                        if (model.Sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                            list = list.OrderByDescending(o => o.Description);
                        else list = list.OrderBy(o => o.Description);
                    if (model.Sort.SortParameter.Equals("Department", StringComparison.OrdinalIgnoreCase))
                        if (model.Sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                            list = list.OrderByDescending(o => o.Department.Name);
                        else list = list.OrderBy(o => o.Department.Name);
                   
                    if (model.Sort.SortParameter.Equals("TotalValue", StringComparison.OrdinalIgnoreCase))
                        if (model.Sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                            list = list.OrderByDescending(o => o.GoodsAndServices.Sum(s => s.Amount + s.TaxAmount));
                        else list = list.OrderBy(o => o.GoodsAndServices.Sum(s => s.Amount + s.TaxAmount));

                    if (model.Sort.SortParameter.Equals("PprfNo", StringComparison.OrdinalIgnoreCase))
                        if (model.Sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                            list = list.OrderByDescending(o => o.PPRFNo)
                                        .ThenByDescending(t => t.PayingEntity.Name)
                                        .ThenByDescending(t => t.Year)
                                        .ThenByDescending(t => t.Month)
                                        .ThenByDescending(t => t.AutoGeneratedSequence);
                        else list = list.OrderBy(o => o.DocumentType)
                                        .ThenBy(t => t.PayingEntity.Name)
                                        .ThenBy(t => t.Year)
                                        .ThenBy(t => t.Month)
                                        .ThenBy(t => t.AutoGeneratedSequence);
                    if (model.Sort.SortParameter.Equals("Status", StringComparison.OrdinalIgnoreCase))
                        if (model.Sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                            list = list.OrderByDescending(o => o.Status);
                        else list = list.OrderBy(o => o.Status);
                    if (model.Sort.SortParameter.Equals("Remarks", StringComparison.OrdinalIgnoreCase))
                        if (model.Sort.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                            list = list.OrderByDescending(o => o.Remarks);
                        else list = list.OrderBy(o => o.Remarks);
                }
                if (model.Filters != null)
                {
                    if (model.Filters.StartDate != null)
                    {
                        model.Filters.StartDate = model.Filters.StartDate.Value.ToStartDate();
                        list = list.Where(w => model.Filters.StartDate.Value <= w.PprfDate);
                    }
                    if (model.Filters.EndDate != null)
                    {
                        model.Filters.EndDate = model.Filters.EndDate.Value.ToEndDate();
                        list = list.Where(w => model.Filters.EndDate.Value >= w.PprfDate);
                    }
                    if (!string.IsNullOrEmpty(model.Filters.Status) && !model.Filters.Status.Equals("All", StringComparison.OrdinalIgnoreCase))
                    {
                        list = list.Where(w => w.Status.ToLower() == model.Filters.Status.ToLower());
                    }
                    if (model.Filters.PayingEntityId != null)
                    {
                        var payingEntityId = model.Filters.PayingEntityId.ToGuid();
                        list = list.Where(w => w.PayingEntityId.Equals(payingEntityId));
                    }
                }

                if (model.Pager == null)
                {
                    model.RequestForms.AddRange(RequestForm.ToModel(list.OrderBy(o => o.CreatedOn).ToList()));
                }
                else
                {
                    model.Pager.TotalItems = list.Count();
                    var pagedList = list//.OrderByDescending(o => o.CreatedOn)
                                    .Skip((model.Pager.CurrentPage - 1) * model.Pager.ItemsPerPage)
                                    .Take(model.Pager.ItemsPerPage).ToList();
                    model.RequestForms.AddRange(RequestForm.ToModel(pagedList));
                }
                response = new ResponseObject<RequestFormsViewModel>
                {
                    Data = model,
                    ResponseType = "success",
                    Message = ""
                };
            }
            return response;
        }

        public RequestFormDetails GetClosedRequest(string requestId)
        {
            RequestFormDetails form = null;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var id = requestId.ToGuid();
                var form1 = _requestFormDAL.List().Where(w => w.Id.Equals(id) && w.Approvers.All(a => a.ApprovalStatus == ApprovalStatus.APPROVED) && w.IsClosed).FirstOrDefault();
                if (form1 != null)
                {
                    form = RequestFormDetails.ToModel(form1);
                }
            }
            return form;
        }

        public RequestFormDetails GetClosedOrRejectedRequest(string requestId)
        {
            RequestFormDetails form = null;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var id = requestId.ToGuid();
                var form1 = _requestFormDAL.List().Where(w => w.Id.Equals(id) && (w.Approvers.Any(a => a.ApprovalStatus == ApprovalStatus.REJECTED) || w.IsClosed)).FirstOrDefault();
                if (form1 != null)
                {
                    form = RequestFormDetails.ToModel(form1);
                }
            }
            return form;
        }

        public RequestFormDetails GetUserRequestById(string requestId)
        {
            RequestFormDetails form = null;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var id = requestId.ToGuid();
                form = RequestFormDetails.ToModel(_requestFormDAL.List().Where(w => w.Id.Equals(id)).FirstOrDefault());
            }
            return form;
        }

        public BudgetSpentInfo GetBudgetSpentInfoById(string BudgetrequestId)
        {
            BudgetSpentInfo spendingInfo = null;

            using (var _requestFormDAL = new RequestFormDAL())
            {
                var id = BudgetrequestId.ToGuid();
                var BudgetList = _requestFormDAL.List().Where(w => (w.BudgetPPRFNoId == id)).ToList();
                spendingInfo = BudgetSpentInfo.ToModel(BudgetList);
            }
            return spendingInfo;
        }

        public async Task<ResponseObject<UpdateRequestFormResponse>> UpdateRequest(UpdateRequestViewModel model)
        {
            var isUpdated = false;
            bool sendEmail = true;
            ResponseObject<UpdateRequestFormResponse> response = null;
            string message = string.Empty, type = string.Empty;
            PaymentRequestForm form = null;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var id = model.RequestId.ToGuid();
                form = _requestFormDAL.List().Where(w => w.Id.Equals(id)).FirstOrDefault();

                var approved = form.Approvers.Where(w => w.ApprovalStatus == ApprovalStatus.APPROVED).ToList();
                var pending = form.Approvers.Where(w => w.ApprovalStatus == ApprovalStatus.PENDING).ToList();
                var isInvalid = approved.Any(a => model.Approvers.Select(s => s.UserGroupId.ToGuid()).Contains(a.ApproverId));

                if (!isInvalid)
                {
                    var approvers = new List<RequestApprover>();
                    if (pending.Count > 0)
                    {
                        if (model.Approvers.Count > 0 && pending.First().ApproverId.ToString().Equals(model.Approvers.First().UserGroupId))
                        {
                            //model.Approvers.Remove(model.Approvers.First());
                            sendEmail = false;
                        }
                    }
                    var totalApprovers = form.Approvers.Where(w => w.ApprovalStatus != ApprovalStatus.PENDING).Count();
                    foreach (var obj in model.Approvers.Select((approver, idx) => new { approver, idx }))
                    {
                        approvers.Add(new RequestApprover
                        {
                            ApproverId = obj.approver.UserGroupId.ToGuid(),
                            ApprovalStatus = ApprovalStatus.PENDING,
                            ApprovalToken = Guid.NewGuid().ToString(),
                            SequenceNo = totalApprovers + obj.idx + 1,
                            PaymentRequestFormId = form.Id
                        });
                    }
                    isUpdated = _requestFormDAL.UpdateApprovers(id, approvers);
                    if (isUpdated)
                    {
                        //sendEmail = false;
                        if (sendEmail)
                        {
                            await SendApproverEmail(form.Id.ToString(), "","","");
                        }
                        type = Constants.SUCCESS;
                        message = "Request updated successfully";
                    }
                    else
                    {
                        type = Constants.ERROR;
                        message = "Something went wrong while updating the request";
                    }
                }
            }
            response = new ResponseObject<UpdateRequestFormResponse>
            {
                ResponseType = type,
                Message = message
            };
            return response;
        }


        public ResponseObject<AskQuestionResponseModel> ApproverAskQuestion(string requestId, string question, string approverEmail)
        {
            ResponseObject<AskQuestionResponseModel> response = null;
            string responseType = string.Empty, responseMessage = string.Empty;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var form = _requestFormDAL.List().Where(w => w.Id.ToString().Equals(requestId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (form != null)
                {
                    var currentApprover = form.Approvers.Where(w => w.Approver.AppUser.Email.Equals(approverEmail)).FirstOrDefault();
                    if (currentApprover != null)
                    {
                        if (currentApprover.ApprovalStatus == ApprovalStatus.PENDING)
                        {
                            if (currentApprover.Queries.Count < 2)
                            {
                                var requestQuestion = new ApproverRequestQuestion
                                {
                                    Question = question,
                                    ApprovalId = currentApprover.Id
                                };
                                var result = _requestFormDAL.ApproverAskQuestion(currentApprover.Id, requestQuestion);
                                if (result)
                                {
                                    responseType = Constants.SUCCESS;
                                    responseMessage = "Your query has been posted successfully.";
                                }
                                else
                                {
                                    responseType = Constants.ERROR;
                                    responseMessage = "Something went wrong while processing your request.";
                                }
                            }
                            else
                            {
                                responseType = Constants.ERROR;
                                responseMessage = "You cannot ask more than 2 questions.";
                            }
                        }
                        else
                        {
                            responseType = Constants.ERROR;
                            responseMessage = "You have already " + currentApprover.ApprovalStatus + " the request.";
                        }
                    }
                    else
                    {
                        responseType = Constants.ERROR;
                        responseMessage = "You are not an approver for this request.";
                    }
                }
                else
                {
                    responseType = Constants.ERROR;
                    responseMessage = "No PO/PPRF request found.";
                }
                response = new ResponseObject<AskQuestionResponseModel>
                {
                    ResponseType = responseType,
                    Message = responseMessage
                };
            }
            return response;
        }

        public ResponseObject<SaveAnswerResponseModel> SaveAnswer(string questionId, string answer, string originatorEmail)
        {
            var result = false;
            ResponseObject<SaveAnswerResponseModel> response = null;
            string responseType = string.Empty, responseMessage = string.Empty;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var questionNotFound = false;
                var form = _requestFormDAL.List().Where(w => w.Approvers.Any(a => a.Queries.Any(aq => aq.Id.ToString().Equals(questionId, StringComparison.OrdinalIgnoreCase)))).FirstOrDefault();
                if (form != null)
                {
                    if (form.Originator.Email.Equals(originatorEmail, StringComparison.OrdinalIgnoreCase))
                    {
                        var approver = form.Approvers.Where(w => w.Queries.Any(a => a.Id.ToString().Equals(questionId, StringComparison.OrdinalIgnoreCase))).FirstOrDefault();
                        if (approver != null)
                        {
                            var question = approver.Queries.Where(w => w.Id.ToString().Equals(questionId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                            if (question != null)
                            {
                                if (question.AnsweredOn == null)
                                {
                                    result = _requestFormDAL.SaveAnswer(questionId, answer);
                                    if (result)
                                    {
                                        responseType = Constants.SUCCESS;
                                        responseMessage = "Answer saved";
                                    }
                                    else
                                    {
                                        responseType = Constants.ERROR;
                                        responseMessage = "Something went wrong while saving the answer.";
                                    }
                                }
                                else
                                {
                                    responseType = Constants.ERROR;
                                    responseMessage = "This question is already answered.";
                                }
                            }
                            else
                            {
                                questionNotFound = true;
                            }
                        }
                        else
                        {
                            questionNotFound = true;
                        }
                        if (questionNotFound)
                        {
                            responseType = Constants.ERROR;
                            responseMessage = "Question details not found.";
                        }
                    }
                    else
                    {
                        responseType = Constants.ERROR;
                        responseMessage = "You cannot answer this question. This was not asked to you.";
                    }
                }
                else
                {
                    responseType = Constants.ERROR;
                    responseMessage = "No such PO/PPRF request found.";
                }
                response = new ResponseObject<SaveAnswerResponseModel>
                {
                    ResponseType = responseType,
                    Message = responseMessage
                };
            }
            return response;
        }


        public ResponseObject<List<QuestionAnswer>> GetRequestQueries(string p)
        {
            ResponseObject<List<QuestionAnswer>> response = new ResponseObject<List<QuestionAnswer>>();
            string responseType = string.Empty, message = string.Empty;
            List<QuestionAnswer> data = null;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var form = _requestFormDAL.List().Where(w => w.Id.ToString().Equals(p, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (form != null)
                {
                    if (form.Approvers.Any(a => a.Queries.Count > 0))
                    {
                        responseType = "success";
                        message = form.Approvers.Sum(s => s.Queries.Count) + " queries found";
                        data = new List<QuestionAnswer>();
                        form.Approvers.ForEach(a =>
                        {
                            a.Queries.ForEach(q =>
                            {
                                data.Add(new QuestionAnswer
                                {
                                    Question = q.Question,
                                    Answer = q.Answer,
                                    ApproverName = a.Approver.AppUser.Name,
                                    AskedOn = q.AskedOn,
                                    AnsweredBy = q.AnsweredOn.HasValue ? form.Originator.Name : string.Empty,
                                    AnsweredOn = q.AnsweredOn
                                });
                            });
                        });
                    }
                    else
                    {
                        responseType = "error";
                        message = "No queries found";
                    }
                }
                else
                {
                    responseType = "error";
                    message = "Request not found.";
                }
            }
            response = new ResponseObject<List<QuestionAnswer>>
            {
                ResponseType = responseType,
                Message = message,
                Data = data
            };
            return response;
        }

        public async Task SendQuestionAskedEmail(string p, string RequestId, string Question)
        {
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var form = _requestFormDAL.List().Where(w => RequestId.Equals(w.Id.ToString(), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (form != null)
                {
                    var approver = form.Approvers.Where(w => w.Approver.AppUser.Email.Equals(p, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (approver != null)
                    {
                        var isSent = await _emailHelper.SendQuestionAskedEmail(new EmailMessage
                        {
                            To = new List<string> { form.Originator.Email },
                            BCC = new List<string> { ConfigurationManager.AppSettings["BCC"] != null ? ConfigurationManager.AppSettings["BCC"] : string.Empty }
                        }, RequestFormDetails.ToModel(form));
                    }
                }
            }
        }

        public RequestForm GetFormByQuestionId(string QuestionId)
        {
            RequestForm _form = null;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var idToSearch = QuestionId.ToGuid();
                _form = RequestForm.ToModel(_requestFormDAL.List().Where(w => w.Approvers.Any(a => a.Queries.Any(q => q.Id.Equals(idToSearch)))).FirstOrDefault());
                //_form = RequestForm.ToModel(_requestFormDAL.List().Where(w => w.Id.Equals(idToSearch)).FirstOrDefault());
            }
            return _form;
        }

        public ResponseObject<CancelRequestResponse> CancelRequest(string requestId, string loggedInUserEmail,string CloseReason,string CloseRemark)
        {
            ResponseObject<CancelRequestResponse> responseObject = null;
            string resultType = string.Empty;
            string message = string.Empty;
            using (var _requestFormDAL = new RequestFormDAL())
            {
                var IdToSearch = requestId.ToGuid();
                var request = RequestFormDetails.ToModel(_requestFormDAL.List().Where(w => w.Id.Equals(IdToSearch)).FirstOrDefault());
                var form = _requestFormDAL.List().Where(w => w.Id.Equals(IdToSearch)).FirstOrDefault();
                if (form != null)
                {
                    if (form.Originator.Email.Equals(loggedInUserEmail, StringComparison.OrdinalIgnoreCase) || form.CreatedBy.Email.Equals(loggedInUserEmail, StringComparison.OrdinalIgnoreCase))
                    {
                        var nottocancel = new string[] { "approved", "closed" };
                        if (nottocancel.Any(a => form.Status.Equals(a, StringComparison.OrdinalIgnoreCase)))
                        {
                            resultType = "error";
                            message = "This request is already approved. You cannot cancel this request";
                        }
                        else
                        {
                            var result = _requestFormDAL.CancelRequest(IdToSearch,  CloseReason,  CloseRemark);
                            if (result)
                            {
                                resultType = "success";
                                message = "Request cancelled successfully";
                            }
                            else
                            {
                                resultType = "error";
                                message = "Something went wrong while cancelling the request";
                            }
                        }
                    }
                    else
                    {
                        resultType = "error";
                        message = "You are not authorized to cancel the request";
                    }
                }
                else
                {
                    resultType = "error";
                    message = "Request not found";
                }
            }
            responseObject = new ResponseObject<CancelRequestResponse>
            {
                ResponseType = resultType,
                Message = message
            };
            return responseObject;
        }
    }
}
