using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebApp.DAL.BAL;
using WebApp.DAL.Data;
using WebApp.DAL.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace WebApp.DAL.Helpers
{
    internal class EmailService
    {
        private MailMessage message;

        public async Task<bool> SendEmail(EmailMessage message)
        {
            var ret = false;
            this.message = new MailMessage();
            this.message.From = new MailAddress(message.From);
            this.message.Subject = message.Subject;
            this.message.IsBodyHtml = true;
            this.message.Body = message.Body;

            message.To.ForEach(f => this.message.To.Add(f));
            message.CC.ForEach(f => this.message.CC.Add(f));
            message.BCC.ForEach(f => this.message.Bcc.Add(f));

            message.Attachments.ForEach(f => this.message.Attachments.Add(new System.Net.Mail.Attachment(f.FileName)
            {
                Name = f.EmailName
            }));

            try
            {

                var apiKey = "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";
                var client = new SendGridClient(apiKey);
                //client.UrlPath = "https://api.sendgrid.com/v3/mail/send";
                 var from = new EmailAddress("noreply@qvivacay.com", "No Reply @ QVI Vacay");
                var subject = message.Subject;              
                var to = new EmailAddress("ramesh.v@qlstyle.com", "Ramesh");  //Temporary
                var plainTextContent = message.Body;
                var htmlContent = message.Body;
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = client.SendEmailAsync(msg);
              
                ret = true;
                //using (MailMessage mailMsg = new MailMessage())
                //{
                //    // API key
                //    string apiKey1 = Environment.GetEnvironmentVariable("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");

                //    // To
                //    mailMsg.To.Add(new MailAddress("ramesh.v@qlstyle.com", "To Name"));

                //    // From
                //    mailMsg.From = new MailAddress("noreply@qvivacay.com", "From Name");

                //    // Subject and multipart/alternative Body
                //    mailMsg.Subject = message.Subject;
                //    string text = message.Body;
                //    string html = @"<p>html body</p>";
                //    mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text));
                //    mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html));

                //    // Init SmtpClient and send
                //    using (SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", 587))
                //    {
                //        smtpClient.Credentials = new NetworkCredential("apikey", apiKey);
                //        smtpClient.Send(mailMsg);
                //    }
                //}

                //Temporary existing codes hide For Local Testing
                //else
                //{
                //    using (var smtpClient = new SmtpClient())
                //    {
                //        smtpClient.Host = ConfigurationManager.AppSettings["SMTP_HOST"];
                //        smtpClient.Port = int.Parse(ConfigurationManager.AppSettings["SMTP_PORT"]);
                //        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                //        smtpClient.UseDefaultCredentials = false;

                //        if (ConfigurationManager.AppSettings["CUSTOM_SMTP"] == "1")  //Use Custom SMTP for testing
                //        {
                //            smtpClient.EnableSsl = true;  //Enable only when testing outside the office
                //            smtpClient.Timeout = 10000;
                //            smtpClient.UseDefaultCredentials = false;
                //            smtpClient.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["CUSTOM_SMTP_USERID"],
                //                                                           ConfigurationManager.AppSettings["CUSTOM_SMTP_PASSWORD"]);
                //        }

                //        await smtpClient.SendMailAsync(this.message).ContinueWith((x) => { return x.IsCompleted; });
                //        ret = true;
                //    }
                //}
            }
            catch (Exception ex)
            {
                ret = false;
            }
            return ret;
        }

        public void Dispose()
        {
            message.Dispose();
        }
    }

    internal class EmailAttachment
    {
        public string FileName { get; set; }
        public string EmailName { get; set; }
    }
    internal class EmailMessage
    {
        public List<string> To { get; set; }
        public List<string> CC { get; set; }
        public List<string> BCC { get; set; }

        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public List<EmailAttachment> Attachments { get; set; }

        public EmailMessage()
        {
            //To = new List<string>();
            CC = new List<string>();
            BCC = new List<string>();
            Attachments = new List<EmailAttachment>();
        }
    }

    internal class ApprovalRequestEmailMessage : EmailMessage
    {
        public EmailToken SecurityToken { get; set; }
    }
    internal class EmailHelper
    {
        private readonly UserBAL _userBAL = new UserBAL();
        private readonly EmailService _emailService = new EmailService();
        private readonly PayingEntityBAL _payingEntityBAL = new PayingEntityBAL();

        public async Task<bool> SendApprovalRequestEmail(ApprovalRequestEmailMessage requestEmail, RequestFormDetails form)
        {
            return await SendApprovalEmail(requestEmail, form, requestEmail.Subject);
        }

        public async Task<bool> SendClarificationRequestEmail(RequestFormDetails form, RequestFormApprovalStatusDetails approver)
        {
            StreamReader sr = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath(@"~/EmailTemplates/03_Clarification.htm"));
            StringBuilder details = new StringBuilder(sr.ReadToEnd());
            sr.Close();

            details.Replace("{{OriginatorName}}", form.Originator.OriginatorName).Replace("{{DocumentType}}", form.DocumentType);
            details.Replace("{{PPRFNo}}", form.PprfNo).Replace("{{NextApprover}}", approver.ApproverName);

            var isSent = await _emailService.SendEmail(new EmailMessage
            {
                To = new List<string> { form.Originator.OriginatorEmail },
                From = ConfigurationManager.AppSettings["FromEmail"].ToString(),
                Subject = "Clarification sought for request " + form.PprfNo,
                Body = details.ToString()
            });
            return isSent;
        }


        public async Task<bool> SendClarificationGivenEmail(RequestFormDetails form, ApprovalRequestEmailMessage requestEmail)
        {
            return await SendApprovalEmail(requestEmail, form, "Clarifications for request " + form.PprfNo);
        }


        private string getRequestFormTemplate(ApprovalRequestEmailMessage requestEmail, RequestFormDetails form)
        {
            StreamReader sr;

            if (form.DocumentType == "PFB")
            {
                sr = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath(@"~/EmailTemplates/NewApprovalRequest/PPRF_New(v3.0)_PFB.html"));
            }
            else if (form.DocumentType == "BDG")
            {
                sr = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath(@"~/EmailTemplates/NewApprovalRequest/PPRF_New(v3.0)_BDG.html"));
            }
            else
            {
                sr = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath(@"~/EmailTemplates/NewApprovalRequest/PPRF_New(v3.0).html"));
            }

            StringBuilder details = new StringBuilder(sr.ReadToEnd());
            sr.Close();
            var logoURL = string.Empty;
            if (!string.IsNullOrEmpty(form.PayingEntityLogoName))
            {
                logoURL = ConfigurationManager.AppSettings["baseURL"] + "/Logos/PayingEntityLogos/" + form.PayingEntityLogoName;
            }

            details.Replace("{{Current Approver}}", form.Approvals.Where(w => w.ApprovalStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase)).OrderBy(o => o.SequenceNumber).FirstOrDefault().ApproverName);
            details.Replace("{{PayingEntityLogo}}", logoURL);

            var monthYear = new DateTime(Convert.ToInt32(form.Year), form.Month, 01);

            details.Replace("{{PPRFNo}}", form.PprfNo);
            details.Replace("{{PPRFDate}}", form.PprfDate.ToString("dd-MMM-yyyy")).Replace("{{PaymentDue}}", form.DueDate.ToString("dd-MMM-yyyy"));

            details.Replace("{{DocumentType}}", form.DocumentType);
            details.Replace("{{DocumentStatus}}", form.Status);

            details.Replace("{{PayingEntityName}}", form.PayingEntityName).Replace("{{Status}}", form.Status);
            details.Replace("{{CountryName}}", form.CountryName).Replace("{{period}}", monthYear.ToString("MMMM - yyyy"));
            details.Replace("{{DepartmentName}}", form.DepartmentName+" (" + form.DepartmentsAccountName +" )").Replace("{{FrequencyTypeName}}", form.FrequencyTypeName);
            
            details.Replace("{{PaymentMethodName}}", form.PaymentMethodName).Replace("{{ExpenseTypeName}}", form.ExpenseTypeName);
            details.Replace("{{PayeeName}}", form.Payee.Name).Replace("{{CurrencyName}}", form.CurrencyType.Name);
            details.Replace("{{PayeeHotelName}}", form.Payee.HotelName).Replace("{{PayeeHotelCountry}}", form.Payee.CountryName);

            if (form.DocumentType == "PFB")
            {
                details.Replace("{{BudgetPPRFNo}}", form.BudgetPPRFNo);
                details.Replace("{{BudgetApprovedAmountUSD}}", form.BudgetApprovedAmtUSDDesc).Replace("{{BudgetApprovedAmount}}", "(" + form.BudgetApprovedAmtDesc + ")");
                details.Replace("{{BudgetSpentAmountUSD}}", form.BudgetSpentAmtUSDDesc);
                details.Replace("{{BudgetRemainingAmountUSD}}", form.BudgetRemainingAmtUSDDesc);

                if (form.BudgetOverSpentDesc != "")
                {
                    details.Replace("{{BudgetOverSpentDesc}}", "(" + form.BudgetOverSpentDesc + ")");
                }
                else
                {
                    details.Replace("{{BudgetOverSpentDesc}}", "");
                }
            }
            else if (form.DocumentType == "BDG")
            {
                details.Replace("{{BudgetValidFrom}}", form.BudgetValidFrom.Value.ToString("dd/MMM/yyyy"));
                details.Replace("{{BudgetValidTo}}", form.BudgetValidTo.Value.ToString("dd/MMM/yyyy"));
                details.Replace("{{RestrictedPayeeOnlyFlag}}", form.RestrictedPayeeOnlyFlag);
            }

            details.Replace("{{Description}}", form.Description).Replace("{{Remarks}}", form.Remarks);
            details.Replace("{{Urgency}}", form.UrgentRemark);
            details.Replace("{{Clarifications}}", form.Clarifications);

            sr = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath(@"~/EmailTemplates/NewApprovalRequest/GoodsAndServiceRow.html"));
            string goodsAndServicesRow = sr.ReadToEnd();
            StringBuilder goodsAndServices = new StringBuilder();
            sr.Close();
            details.Replace("{{CurrencyCode}}", form.CurrencyType.Code);

            details.Replace("{{USDExRate}}", form.USDExRate.ToCurrencyString(5));
            details.Replace("{{EURExRate}}", form.EuroExRate.ToCurrencyString(5));

            foreach (var row in form.GoodsAndServices.Select((service, idx) => new { idx, service }))
            {
                StringBuilder serviceRow = new StringBuilder(goodsAndServicesRow);
                serviceRow.Replace("{{indexno}}", (row.idx + 1).ToString());
                serviceRow.Replace("{{Description}}", row.service.Description)
                           .Replace("{{TaxTypeName}}", row.service.TaxType.Name);
                serviceRow.Replace("{{Quantity}}", string.Format("{0:0}", row.service.Quantity.ToString()))
                           .Replace("{{Amount}}", row.service.Amount.ToCurrencyString());
                serviceRow.Replace("{{AmountUSD}}", (row.service.Amount * form.CurrencyType.USDValue).ToCurrencyString());
                serviceRow.Replace("{{AmountEuro}}", (row.service.Amount * form.CurrencyType.EuroValue).ToCurrencyString());
                goodsAndServices.Append(serviceRow);
            }
            details.Replace("{{goodsAndServicesRows}}", goodsAndServices.ToString());
            details.Replace("{{SubTotal}}", form.SubTotal.ToCurrencyString());
            details.Replace("{{SubTotalUSD}}", (form.SubTotal * form.CurrencyType.USDValue).ToCurrencyString());
            details.Replace("{{SubTotalEuro}}", (form.SubTotal * form.CurrencyType.EuroValue).ToCurrencyString());

            details.Replace("{{TaxType2Name}}", form.TaxType2Name).Replace("{{TaxType3Name}}", form.TaxType3Name);
            details.Replace("{{TaxAmount}}", form.TaxAmount1.ToCurrencyString());
            details.Replace("{{TaxAmountUSD}}", (form.TaxAmount1 * form.CurrencyType.USDValue).ToCurrencyString());
            details.Replace("{{TaxAmountEuro}}", (form.TaxAmount1 * form.CurrencyType.EuroValue).ToCurrencyString());
            details.Replace("{{TaxAmount2}}", form.TaxAmount2.ToCurrencyString())
                   .Replace("{{TaxAmount2USD}}", (form.TaxAmount2 * form.CurrencyType.USDValue).ToCurrencyString())
                   .Replace("{{TaxAmount2Euro}}", (form.TaxAmount2 * form.CurrencyType.EuroValue).ToCurrencyString());
            details.Replace("{{TaxAmount3}}", form.TaxAmount3.ToCurrencyString())
                   .Replace("{{TaxAmount3USD}}", (form.TaxAmount3 * form.CurrencyType.USDValue).ToCurrencyString())
                   .Replace("{{TaxAmount3Euro}}", (form.TaxAmount3 * form.CurrencyType.EuroValue).ToCurrencyString());
            details.Replace("{{Total}}", form.Total.ToCurrencyString())
                   .Replace("{{TotalUSD}}", (form.Total * form.CurrencyType.USDValue).ToCurrencyString())
                   .Replace("{{TotalEuro}}", (form.Total * form.CurrencyType.EuroValue).ToCurrencyString());

            details.Replace("{{OriginatorName}}", form.Originator.OriginatorName).Replace("{{OriginatorDesignation}}", form.Originator.OriginatorDesignation);
            details.Replace("{{OriginatorDepartmentName}}", form.Originator.OriginatorDepartmentName).Replace("{{OriginatorStatus}}", form.Originator.OriginatorStatus);

            sr = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath(@"~/EmailTemplates/NewApprovalRequest/ApproverRow.html"));
            string approverRowTemplate = sr.ReadToEnd();
            StringBuilder approvers = new StringBuilder();
            sr.Close();
            sr = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath(@"~/EmailTemplates/NewApprovalRequest/QuestionAndAnswerRow.html"));
            string queryRowTemplate = sr.ReadToEnd();
            var queries = new StringBuilder();
            sr.Close();
            int questionCount = 0;
            foreach (var row in form.Approvals.Select((approval, idx) => new { idx, approval }))
            {
                StringBuilder approverRow = new StringBuilder(approverRowTemplate);
                approverRow.Replace("{{indexno}}", (row.idx + 1).ToString());
                approverRow.Replace("{{ApproverName}}", row.approval.ApproverName);
                approverRow.Replace("{{ApproverDesignation}}", row.approval.ApproverDesignation);
                approverRow.Replace("{{ApproverDepartmentName}}", row.approval.ApproverDepartmentName);
                approverRow.Replace("{{ApprovalStatus}}", row.approval.ApprovalStatus);
                approverRow.Replace("{{ApprovalResponseDate}}", row.approval.ResponseDate.HasValue ? row.approval.ResponseDate.Value.ToString("dd/MM/yyyy") : string.Empty);
                approverRow.Replace("{{Remarks}}", row.approval.Remarks);
                approvers.Append(approverRow);

                foreach (var row1 in row.approval.Questions.OrderBy(o => o.AskedOn).Select((query, idx) => new { idx, query }))
                {
                    var queryRow = new StringBuilder(queryRowTemplate);
                    queryRow.Replace("{{indexno}}", (++questionCount).ToString());
                    queryRow.Replace("{{ApproverName}}", row.approval.ApproverName);
                    queryRow.Replace("{{Question}}", row1.query.Question);
                    queryRow.Replace("{{AskedOn}}", row1.query.AskedOn);
                    queryRow.Replace("{{AnsweredOn}}", row1.query.AnsweredOn);
                    queryRow.Replace("{{Answer}}", row1.query.Answer);
                    queries.Append(queryRow);
                }
            }
            details.Replace("{{approverRows}}", approvers.ToString());
            details.Replace("{{QuestionAndAnswerRows}}", queries.ToString());


            details.Replace("{{payeeBankName}}", form.PayeeBankDetails.BankName).Replace("{{payeeAccountName}}", form.PayeeBankDetails.AccountName);
            details.Replace("{{payeeAddressLine1}}", form.Payee.AddressLine1).Replace("{{payeeAddressLine2}}", form.Payee.AddressLine2);
            details.Replace("{{payeeAddressLine3}}", form.Payee.AddressLine3).Replace("{{payeeAccountNumber}}", form.PayeeBankDetails.AccountNumber);
            details.Replace("{{payeeAccountType}}", form.PayeeBankDetails.AccountType).Replace("{{payeeIBAN}}", form.PayeeBankDetails.IBAN);
            details.Replace("{{payeeSwift}}", form.PayeeBankDetails.Swift).Replace("{{payeeIFSC}}", form.PayeeBankDetails.IFSC);

            details.Replace("{{baseurlToApproveOrReject}}", ConfigurationManager.AppSettings["baseurlToApproveOrRejectFromEmail"].ToString());
            details.Replace("{{token}}", HttpUtility.UrlEncode(requestEmail.SecurityToken.Token)).Replace("{{lengths}}", HttpUtility.UrlEncode(requestEmail.SecurityToken.Lengths));

            var x = details.ToString();
            return details.ToString();
        }

        internal async Task<bool> SendAppovalSuccessfulEmail(RequestFormDetails form)
        {
            StreamReader sr = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath(@"~/EmailTemplates/01_Approved_Notification.htm"));
            StringBuilder details = new StringBuilder(sr.ReadToEnd());
            sr.Close();

            details.Replace("{{OriginatorName}}", form.Originator.OriginatorName).Replace("{{DocumentType}}", form.DocumentType);
            details.Replace("{{PayeeName}}", form.Payee.Name);
            details.Replace("{{Description}}", form.Description);
            details.Replace("{{PPRFNo}}", form.PprfNo);
            details.Replace("{{Amount}}", form.Total+"/"+form.TotalUSD+" (USD)");

            var isSent = await _emailService.SendEmail(new EmailMessage
            {
                From = ConfigurationManager.AppSettings["FromEmail"].ToString(),
                To = new List<string> { form.Originator.OriginatorEmail },
                CC = new List<string> { ConfigurationManager.AppSettings["FINANCE_EMAIL"] },
                BCC = new List<string> { ConfigurationManager.AppSettings["BCC"] },
                Subject = "Request: " + form.PprfNo + " approved",
                Body = details.ToString()
            });

            var body = getApprovedPPRFTemplate(form);
            var emails = _payingEntityBAL.GetEmailsForEntityWithRange(form.PayingEntityCode, form.TotalUSD);
            var isSent2 = await _emailService.SendEmail(new EmailMessage
            {
                From = ConfigurationManager.AppSettings["FromEmail"].ToString(),
                To = emails,
                Subject = "Request: " + form.PprfNo + " approved",
                Body = body
            });

            return isSent;
        }

        internal async Task<bool> SendPPRFRejectedEmail(RequestFormDetails form,string Remark)
        {
            StreamReader sr = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath(@"~/EmailTemplates/02_Rejection_Notification.htm"));
            StringBuilder details = new StringBuilder(sr.ReadToEnd());
            sr.Close();

            details.Replace("{{OriginatorName}}", form.Originator.OriginatorName).Replace("{{DocumentType}}", form.DocumentType);
            details.Replace("{{PPRFNo}}", form.PprfNo);
            details.Replace("{{Description}}", form.Description);
            details.Replace("{{Remark}}", Remark);           
            details.Replace("{{CurrentApprover}}", form.Approvals.Where(w => w.ApprovalStatus.Equals("Rejected")).FirstOrDefault().ApproverName);

            var isSent = await _emailService.SendEmail(new EmailMessage
            {
                From = ConfigurationManager.AppSettings["FromEmail"].ToString(),
                To = new List<string> { form.Originator.OriginatorEmail },
                CC = new List<string> { ConfigurationManager.AppSettings["FINANCE_EMAIL"] },
                BCC = new List<string> { ConfigurationManager.AppSettings["BCC"] },
                Subject = $"Request {form.PprfNo} has been rejected",
                Body = details.ToString()
            });

            return isSent;
        }

        private async Task<bool> SendApprovalEmail(ApprovalRequestEmailMessage requestEmail, RequestFormDetails form, string subject)
        {
            var isSent = false;
            byte[] file = null;
            var extension = string.Empty;
            string fullname = null;

            try
            {
                var attachments = new List<EmailAttachment>();

                if (form.PaymentRequestDocuments != null)
                {
                    var docPath = ConfigurationManager.AppSettings["DocumentsPath"] + Path.DirectorySeparatorChar;

                    foreach (var doc in form.PaymentRequestDocuments)
                    {
                        fullname = docPath + doc.DocumentSavedName;

                        attachments.Add(new EmailAttachment
                        {
                            FileName = fullname,
                            EmailName = doc.DocumentName
                        });
                    }
                }

                //          if (!string.IsNullOrEmpty(form.DocumentSavedName))
                //          {
                //              fullname = ConfigurationManager.AppSettings["DocumentsPath"] + Path.DirectorySeparatorChar + form.DocumentSavedName;
                //              file = File.ReadAllBytes(fullname);
                //              extension = Path.GetExtension(fullname);
                //          }
                //
                //          var attachments = new List<EmailAttachment>();
                //          if(!string.IsNullOrEmpty(fullname) && !string.IsNullOrEmpty(form.DocumentName))
                //          {
                //              attachments.Add(new EmailAttachment {
                //                  FileName = fullname,
                //                  EmailName = form.DocumentName
                //              });
                //          }

                isSent = await _emailService.SendEmail(new EmailMessage
                {
                    From = ConfigurationManager.AppSettings["FromEmail"].ToString(),
                    To = requestEmail.To,
                    CC = requestEmail.CC,
                    BCC = requestEmail.BCC,
                    Subject = subject,
                    Body = getRequestFormTemplate(requestEmail, form),
                    Attachments = attachments
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return isSent;
        }

        internal async Task<bool> SendNotificationToOriginator(EmailMessage emailMessage, RequestFormDetails form)
        {
            var isSent = false;
            try
            {
                var subject = "Approval In Progress " + form.DocumentType + " Request No: " + form.PprfNo + " is being reviewed by " + form.Approvals.Where(w => w.ApprovalStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase)).OrderBy(o => o.SequenceNumber).FirstOrDefault().ApproverName;
                isSent = await _emailService.SendEmail(new EmailMessage
                {
                    From = ConfigurationManager.AppSettings["FromEmail"].ToString(),
                    To = emailMessage.To,
                    CC = emailMessage.CC,
                    BCC = emailMessage.BCC,
                    Subject = subject,
                    Body = GetOriginatorNotificationTemplate(form)
                });
            }
            catch (Exception)
            {

            }
            return isSent;
        }

        private string GetOriginatorNotificationTemplate(RequestFormDetails form)
        {
            StreamReader sr = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath(@"~/EmailTemplates/OriginatorNotification.html"));
            StringBuilder details = new StringBuilder(sr.ReadToEnd());
            sr.Close();

            details.Replace("{{PPRFNo}}", form.PprfNo);
            details.Replace("{{OriginatorName}}", form.Originator.OriginatorName);
            details.Replace("{{DocumentType}}", form.DocumentType);
            details.Replace("{{CurrentApprover}}", form.Approvals.Where(w => w.ApprovalStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase)).OrderBy(o => o.SequenceNumber).FirstOrDefault().ApproverName);

            details.Replace("{{HotelName}}", form.Payee.HotelName);
            details.Replace("{{PayeeName}}", form.Payee.Name);
            var totalAmount = 0.0m;
            form.GoodsAndServices.ForEach(f =>
            {
                totalAmount += f.AmountUSD + (f.AmountUSD * f.TaxType.PercentageValue / 100);
            });
            details.Replace("{{TotalAmount}}", totalAmount.ToCurrencyString());
            details.Replace("{{Description}}", form.Description);
            details.Replace("{{Remarks}}", form.Remarks);
            return details.ToString();
        }

        internal async Task<bool> SendQuestionAskedEmail(EmailMessage emailMessage, RequestFormDetails form)
        {
            EmailWebService.ASPEmailWSSoapClient emailClient = new EmailWebService.ASPEmailWSSoapClient();
            var isSent = false;
            try
            {
                var subject = "Approver asked a question " + form.DocumentType + " Request No: " + form.PprfNo + " is being reviewed by " + form.Approvals.Where(w => w.ApprovalStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase)).OrderBy(o => o.SequenceNumber).FirstOrDefault().ApproverName;

                isSent = await _emailService.SendEmail(new EmailMessage
                {
                    From = ConfigurationManager.AppSettings["FromEmail"].ToString(),
                    To = emailMessage.To,
                    CC = emailMessage.CC,
                    BCC = emailMessage.BCC,
                    Subject = subject,
                    Body = GetQuestionAskedNotificationTemplate(form)
                });
            }
            catch (Exception)
            {

            }
            return isSent;
        }

        private string GetQuestionAskedNotificationTemplate(RequestFormDetails form)
        {
            StreamReader sr = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath(@"~/EmailTemplates/QuestionAsked.html"));
            StringBuilder details = new StringBuilder(sr.ReadToEnd());
            sr.Close();

            details.Replace("{{PPRFNo}}", form.PprfNo);
            details.Replace("{{OriginatorName}}", form.Originator.OriginatorName);
            details.Replace("{{DocumentType}}", form.DocumentType);
            details.Replace("{{CurrentApprover}}", form.Approvals.Where(w => w.ApprovalStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase)).OrderBy(o => o.SequenceNumber).FirstOrDefault().ApproverName);
            return details.ToString();
        }

        private string getApprovedPPRFTemplate(RequestFormDetails form)
        {
            StreamReader sr = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath(@"~/EmailTemplates/ApprovedPPRF/Request.html"));
            StringBuilder details = new StringBuilder(sr.ReadToEnd());
            sr.Close();
            var logoURL = string.Empty;
            if (!string.IsNullOrEmpty(form.PayingEntityLogoName))
            {
                logoURL = ConfigurationManager.AppSettings["baseURL"] + "/Logos/PayingEntityLogos/" + form.PayingEntityLogoName;
            }


            details.Replace("{{PayingEntityLogo}}", logoURL);

            var monthYear = new DateTime(Convert.ToInt32(form.Year), form.Month, 01);

            details.Replace("{{PPRFNo}}", form.PprfNo);
            details.Replace("{{PPRFDate}}", form.PprfDate.ToString("dd-MMM-yyyy")).Replace("{{PaymentDue}}", form.DueDate.ToString("dd-MMM-yyyy"));

            details.Replace("{{DocumentType}}", form.DocumentType);
            details.Replace("{{DocumentStatus}}", form.Status);

            details.Replace("{{PayingEntityName}}", form.PayingEntityName).Replace("{{Status}}", form.Status);
            details.Replace("{{CountryName}}", form.CountryName).Replace("{{period}}", monthYear.ToString("MMMM - yyyy"));
            details.Replace("{{DepartmentName}}", form.DepartmentName).Replace("{{FrequencyTypeName}}", form.FrequencyTypeName);
            details.Replace("{{PaymentMethodName}}", form.PaymentMethodName).Replace("{{ExpenseTypeName}}", form.ExpenseTypeName);
            details.Replace("{{PayeeName}}", form.Payee.Name).Replace("{{CurrencyName}}", form.CurrencyType.Name);
            details.Replace("{{PayeeHotelName}}", form.Payee.HotelName).Replace("{{PayeeHotelCountry}}", form.Payee.CountryName);


            details.Replace("{{Description}}", form.Description).Replace("{{Remarks}}", form.Remarks);
            details.Replace("{{Clarifications}}", form.Clarifications);




            sr = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath(@"~/EmailTemplates/ApprovedPPRF/GoodsAndServiceRow.html"));
            string goodsAndServicesRow = sr.ReadToEnd();
            StringBuilder goodsAndServices = new StringBuilder();
            sr.Close();
            details.Replace("{{CurrencyCode}}", form.CurrencyType.Code);

            details.Replace("{{USDExRate}}", form.USDExRate.ToCurrencyString(5));
            details.Replace("{{EURExRate}}", form.EuroExRate.ToCurrencyString(5));

            foreach (var row in form.GoodsAndServices.Select((service, idx) => new { idx, service }))
            {
                StringBuilder serviceRow = new StringBuilder(goodsAndServicesRow);
                serviceRow.Replace("{{indexno}}", (row.idx + 1).ToString());
                serviceRow.Replace("{{Description}}", row.service.Description)
                           .Replace("{{TaxTypeName}}", row.service.TaxType.Name);
                serviceRow.Replace("{{Quantity}}", string.Format("{0:0}", row.service.Quantity.ToString()))
                           .Replace("{{Amount}}", row.service.Amount.ToCurrencyString());
                serviceRow.Replace("{{AmountUSD}}", (row.service.Amount * form.CurrencyType.USDValue).ToCurrencyString());
                serviceRow.Replace("{{AmountEuro}}", (row.service.Amount * form.CurrencyType.EuroValue).ToCurrencyString());
                goodsAndServices.Append(serviceRow);
            }
            details.Replace("{{goodsAndServicesRows}}", goodsAndServices.ToString());
            details.Replace("{{SubTotal}}", form.SubTotal.ToCurrencyString());
            details.Replace("{{SubTotalUSD}}", (form.SubTotal * form.CurrencyType.USDValue).ToCurrencyString());
            details.Replace("{{SubTotalEuro}}", (form.SubTotal * form.CurrencyType.EuroValue).ToCurrencyString());

            details.Replace("{{TaxType2Name}}", form.TaxType2Name).Replace("{{TaxType3Name}}", form.TaxType3Name);
            details.Replace("{{TaxAmount}}", form.TaxAmount1.ToCurrencyString());
            details.Replace("{{TaxAmountUSD}}", (form.TaxAmount1 * form.CurrencyType.USDValue).ToCurrencyString());
            details.Replace("{{TaxAmountEuro}}", (form.TaxAmount1 * form.CurrencyType.EuroValue).ToCurrencyString());
            details.Replace("{{TaxAmount2}}", form.TaxAmount2.ToCurrencyString())
                   .Replace("{{TaxAmount2USD}}", (form.TaxAmount2 * form.CurrencyType.USDValue).ToCurrencyString())
                   .Replace("{{TaxAmount2Euro}}", (form.TaxAmount2 * form.CurrencyType.EuroValue).ToCurrencyString());
            details.Replace("{{TaxAmount3}}", form.TaxAmount3.ToCurrencyString())
                   .Replace("{{TaxAmount3USD}}", (form.TaxAmount3 * form.CurrencyType.USDValue).ToCurrencyString())
                   .Replace("{{TaxAmount3Euro}}", (form.TaxAmount3 * form.CurrencyType.EuroValue).ToCurrencyString());
            details.Replace("{{Total}}", form.Total.ToCurrencyString())
                   .Replace("{{TotalUSD}}", (form.Total * form.CurrencyType.USDValue).ToCurrencyString())
                   .Replace("{{TotalEuro}}", (form.Total * form.CurrencyType.EuroValue).ToCurrencyString());




            details.Replace("{{OriginatorName}}", form.Originator.OriginatorName).Replace("{{OriginatorDesignation}}", form.Originator.OriginatorDesignation);
            details.Replace("{{OriginatorDepartmentName}}", form.Originator.OriginatorDepartmentName).Replace("{{OriginatorStatus}}", form.Originator.OriginatorStatus);



            sr = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath(@"~/EmailTemplates/ApprovedPPRF/ApproverRow.html"));
            string approverRowTemplate = sr.ReadToEnd();
            StringBuilder approvers = new StringBuilder();
            sr.Close();
            sr = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath(@"~/EmailTemplates/ApprovedPPRF/QuestionAndAnswerRow.html"));
            string queryRowTemplate = sr.ReadToEnd();
            var queries = new StringBuilder();
            sr.Close();
            int questionCount = 0;
            foreach (var row in form.Approvals.Select((approval, idx) => new { idx, approval }))
            {
                StringBuilder approverRow = new StringBuilder(approverRowTemplate);
                approverRow.Replace("{{indexno}}", (row.idx + 1).ToString());
                approverRow.Replace("{{ApproverName}}", row.approval.ApproverName);
                approverRow.Replace("{{ApproverDesignation}}", row.approval.ApproverDesignation);
                approverRow.Replace("{{ApproverDepartmentName}}", row.approval.ApproverDepartmentName);
                approverRow.Replace("{{ApprovalStatus}}", row.approval.ApprovalStatus);
                approverRow.Replace("{{ApprovalResponseDate}}", row.approval.ResponseDate.HasValue ? row.approval.ResponseDate.Value.ToString("dd/MM/yyyy") : string.Empty);
                approverRow.Replace("{{Remarks}}", row.approval.Remarks);
                approvers.Append(approverRow);

                foreach (var row1 in row.approval.Questions.OrderBy(o => o.AskedOn).Select((query, idx) => new { idx, query }))
                {
                    var queryRow = new StringBuilder(queryRowTemplate);
                    queryRow.Replace("{{indexno}}", (++questionCount).ToString());
                    queryRow.Replace("{{ApproverName}}", row.approval.ApproverName);
                    queryRow.Replace("{{Question}}", row1.query.Question);
                    queryRow.Replace("{{AskedOn}}", row1.query.AskedOn);
                    queryRow.Replace("{{AnsweredOn}}", row1.query.AnsweredOn);
                    queryRow.Replace("{{Answer}}", row1.query.Answer);
                    queries.Append(queryRow);
                }
            }
            details.Replace("{{approverRows}}", approvers.ToString());
            details.Replace("{{QuestionAndAnswerRows}}", queries.ToString());


            details.Replace("{{payeeBankName}}", form.PayeeBankDetails.BankName).Replace("{{payeeAccountName}}", form.PayeeBankDetails.AccountName);
            details.Replace("{{payeeAddressLine1}}", form.Payee.AddressLine1).Replace("{{payeeAddressLine2}}", form.Payee.AddressLine2);
            details.Replace("{{payeeAddressLine3}}", form.Payee.AddressLine3).Replace("{{payeeAccountNumber}}", form.PayeeBankDetails.AccountNumber);
            details.Replace("{{payeeAccountType}}", form.PayeeBankDetails.AccountType).Replace("{{payeeIBAN}}", form.PayeeBankDetails.IBAN);
            details.Replace("{{payeeSwift}}", form.PayeeBankDetails.Swift).Replace("{{payeeIFSC}}", form.PayeeBankDetails.IFSC);

            return details.ToString();
        }
    }
}
