using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebApp.DAL.Data;
using WebApp.DAL.Helpers;

namespace WebApp.DAL.Models
{
    public class PaymentRequestFormViewModel
    {
        public List<PayingEntity> PayingEntities { get; set; }
        public List<Country> Countries { get; set; }
        public List<Department> Departments { get; set; }
        public List<DepartmentsAccount> DepartmentsAccounts { get; set; }
        public List<FrequencyType> FrequencyTypes { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }
        public List<ExpenseType> ExpenseTypes { get; set; }
        public List<PayeeModel> Payees { get; set; }
        public List<CurrencyType> CurrencyTypes { get; set; }
        public List<TaxType> TaxTypes { get; set; }
        public List<ApproverGroup> Operators { get; set; }
        public List<BudgetOrder> BudgetOrders { get; set; }

        public HttpPostedFileBase Document { get; set; }

    }

    public class SavePaymentProcessingRequestFormViewModel
    {
        public string DocumentType { get; set; }
        public PayingEntity PayingEntity { get; set; }
        public DateTime PPRFDate { get; set; }
        public DateTime DueDate { get; set; }
        public Country Country { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public Department Department { get; set; }

        public DepartmentsAccount DepartmentsAccount { get; set; }
        public FrequencyType FrequencyType { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public ExpenseType ExpenseType { get; set; }
        public PayeeModel Payee { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public CurrencyType CurrencyType { get; set; }


        public decimal USDExRate { get; set; }
        public decimal EuroExRate { get; set; }


        public List<GoodsAndService> GoodsAndServices { get; set; }
        public TaxType TaxType2 { get; set; }
        public decimal TaxAmount2 { get; set; }
        public TaxType TaxType3 { get; set; }
        public decimal TaxAmount3 { get; set; }


        public List<FileAttachment> FileAttachmentsDetails { get; set; }
        public List<HttpPostedFileBase> Files { get; set; }

        public HttpPostedFileBase Document { get; set; }
        //public HttpPostedFileBase Document2 { get; set; }
        //public HttpPostedFileBase Document3 { get; set; }


        public User Originator { get; set; }
        public List<ApproverUser> Approvers { get; set; }
        public PayeeBankDetails PayeeBank { get; set; }

        public BudgetOrder BudgetOrder { get; set; }
        public DateTime? BudgetValidFrom { get; set; }
        public DateTime? BudgetValidTo { get; set; }
        public decimal BudgetApprovedAmt { get; set; }
        public decimal BudgetApprovedAmtUSD { get; set; }
        public decimal BudgetApprovedAmtEuro { get; set; }
        public decimal BudgetSpentAmt { get; set; }
        public decimal BudgetSpentAmtUSD { get; set; }
        public decimal BudgetSpentAmtEuro { get; set; }
        public char RestrictedPayeeOnly { get; set; }

        public string CreatedByEmail { get; set; }
        public string RequestId { get; set; }
        
    }

    public class FileAttachment
    {
        public int Index { get; set; }
    }
    public class GoodsAndService
    {
        public string Description { get; set; }
        public TaxType TaxType { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountUSD { get; internal set; }
        public decimal AmountEuro { get; internal set; }
        public string TaxTypeId { get; set; }
        
    }

    public class BudgetSpentInfo
    {
        public decimal TotalSpentAmountUSD { get; set; }
        public decimal TotalSpentAmountEuro { get; set; }

        internal static BudgetSpentInfo ToModel(List<PaymentRequestForm> bg)
        {
            var spendingDetails = new BudgetSpentInfo();
            spendingDetails.TotalSpentAmountUSD = 0;
            spendingDetails.TotalSpentAmountEuro = 0;

            bg.ForEach(r =>
            {
                spendingDetails.TotalSpentAmountUSD = spendingDetails.TotalSpentAmountUSD + r.GoodsAndServices.Sum(su => su.AmountUSD + su.TaxAmountUSD) + r.Tax2AmountUSD + r.Tax3AmountUSD;
                spendingDetails.TotalSpentAmountEuro = spendingDetails.TotalSpentAmountEuro + r.GoodsAndServices.Sum(su => su.AmountEuro + su.TaxAmountEuro) + r.Tax2AmountEuro + r.Tax3AmountEuro;
            });
        
            //spendingDetails.TotalSpentAmountEuro = requestForm.GoodsAndServices.Sum(su => su.AmountEuro + su.TaxAmountEuro) + requestForm.Tax2AmountEuro + requestForm.Tax3AmountEuro;

            return spendingDetails;
        }
    }

    public class PaymentRequestDocs
    {
        public string Id { get; set; }
        public string DocumentName { get; set; }
        public string DocumentSavedName { get; set; }
    }

    public class UpdateRequestViewModel
    {
        public string RequestId { get; set; }
        public List<ApproverUser> Approvers { get; set; }

        public UpdateRequestViewModel()
        {
            Approvers = new List<ApproverUser>();
        }
    }

    public class ApplicationGroupModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<User> Operators { get; set; }
    }




    public class RequestFormExportExcel
    {
        public string DocumentType { get; set; }
        public string PPRFNo { get; set; }
        public string PPRFDate { get; set; }
        public string DueDate { get; set; }
        public string DocumentStatus { get; set; }
        public string PayingEntityName { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentsAccount { get; internal set; }
        public string OriginatorName { get; set; }
        public string PayeeName { get; set; }
        public string Description { get; set; }
        public decimal? TotalValue { get; set; }
    }


    public class RequestQuestion
    {
        public string Id { get; set; }
        public string Question { get; set; }
    }
    public class QuestionAndAnswer
    {
        public string Question { get; internal set; }
        public string Answer { get; internal set; }

        public string AskedOn { get; internal set; }

        public string AnsweredOn { get; internal set; }
    }
    public class RequestForm
    {
        public string Id { get; set; }
        public string OriginatorName { get; set; }
        public string OriginatorEmail { get; set; }
        public DateTime PprfDate { get; set; }
        public DateTime DueDate { get; set; }

        public User Originator { get; set; }
        public DateTime CreatedOn { get; internal set; }
        public string PayingEntityName { get; internal set; }
        public string PayeeName { get; internal set; }
        public string DepartmentName { get; internal set; }
        public string DepartmentsAccount { get; internal set; }
        public bool IsClarificationRequired { get; private set; }
        public string Clarifications { get; private set; }

        public bool AskAnotherQuestion { get; internal set; }
        public bool IsQuestionAsked { get; internal set; }
        public List<RequestQuestion> RequestQuestions { get; internal set; }

        public string Description { get; private set; }
        public string PayingEntityCode { get; private set; }
        public int Month { get; private set; }
        public int Year { get; private set; }
        public int Number { get; private set; }
        public string Status { get; private set; }
        public string DocumentType { get; private set; }
        public decimal? TotalValue { get; internal set; }
        public decimal? TotalValueInUSD { get; internal set; }
        public decimal? TotalValueIncTax { get; internal set; }
        public decimal? TotalValueIncTaxInUSD { get; internal set; }
        public decimal? TotalTax { get; internal set; }
        public decimal? TotalTaxInUSD { get; internal set; }
        public string CurrencyCode { get; internal set; }
        public string PprfNo { get; internal set; }
        public string BudgetOrderNo { get; set; }
        public DateTime? BudgetValidFrom { get; set; }
        public DateTime? BudgetValidTo { get; set; }
        public decimal BudgetApprovedAmt { get; set; }
        public decimal BudgetApprovedAmtUSD { get; set; }
        public decimal BudgetApprovedAmtEuro { get; set; }
        public decimal BudgetSpentAmt { get; set; }
        public decimal BudgetSpentAmtUSD { get; set; }
        public decimal BudgetSpentAmtEuro { get; set; }
        public bool RestrictedPayeeOnly { get; set; }
        public string Note { get; set; }
        public string Remarks { get; set; }


        internal static RequestForm ToModel(Data.PaymentRequestForm entity)
        {
            if (entity == null)
                return null;
            var requestForm = new RequestForm();
            requestForm.Id = entity.Id.ToString();
            requestForm.DocumentType = entity.DocumentType;
            requestForm.OriginatorName = entity.Originator.Name;
            requestForm.OriginatorEmail = entity.Originator.Email;
            requestForm.Clarifications = entity.Clarifications;
            requestForm.Description = entity.Description;
            requestForm.PprfDate = entity.PprfDate;
            requestForm.DueDate = entity.DueDate;

            if (entity.DocumentType == "BDG" && entity.RestrictedPayeeOnly == false)
            {
                requestForm.PayeeName = "-";
            }
            else
            {
                requestForm.PayeeName = entity.PayeeBankAccountDetail.Payee.Name;
            }

            requestForm.PayingEntityName = entity.PayingEntity.Name;
            requestForm.PayingEntityCode = entity.PayingEntity.Abbreviation;
            requestForm.DepartmentName = entity.Department.Name;
            requestForm.DepartmentsAccount = entity.DepartmentsAccount.Name;



            requestForm.IsClarificationRequired = entity.ClarificationRequired;
            requestForm.PprfNo = entity.PPRFNo;
            requestForm.Month = entity.Month;
            requestForm.Year = entity.Year;
            requestForm.Number = entity.AutoGeneratedSequence;
            requestForm.Status = entity.Status;
            //requestForm.Status = entity.Approvers.Any(a => a.ApprovalStatus.Equals(ApprovalStatus.REJECTED)) ? "Rejected" :
            //                                            entity.Approvers.Any(a => a.ApprovalStatus.Equals(ApprovalStatus.PENDING)) ? "Pending" :
            //                                            entity.Approvers.All(a => a.ApprovalStatus.Equals(ApprovalStatus.APPROVED)) ? "Approved" : "Unknown";
            requestForm.TotalValue = entity.GoodsAndServices.Sum(s => s.Amount);
            requestForm.TotalValueInUSD = entity.GoodsAndServices.Sum(s => s.AmountUSD);
            requestForm.TotalValueIncTax = entity.GoodsAndServices.Sum(s => s.Amount + s.TaxAmount);
            requestForm.TotalValueIncTaxInUSD = entity.GoodsAndServices.Sum(s => s.AmountUSD + s.TaxAmountUSD);
            requestForm.TotalTax = entity.GoodsAndServices.Sum(s => s.TaxAmount);
            requestForm.TotalTaxInUSD = entity.GoodsAndServices.Sum(s => s.TaxAmountUSD);
            requestForm.Originator = new User
            {
                Name = entity.Originator.Name,
                Department = new Department
                {
                    Name = entity.Department.Name,
                    Code = entity.Department.Code
                },
                Email = entity.Originator.Email,
                Designation = entity.Originator.Designation,
            };

            requestForm.CurrencyCode = entity.Currency.Code;

            if (entity.BudgetPPRFNo != null)
            {
                requestForm.BudgetOrderNo = entity.BudgetPPRFNo.PPRFNo;
            }
            else
            {
                requestForm.BudgetOrderNo = "-";
            }
                        
            requestForm.BudgetValidFrom = entity.BudgetValidFrom;
            requestForm.BudgetValidTo = entity.BudgetValidTo;
            requestForm.RestrictedPayeeOnly = entity.RestrictedPayeeOnly;
            requestForm.BudgetApprovedAmt = entity.BudgetApprovedAmt;
            requestForm.BudgetApprovedAmtUSD = entity.BudgetApprovedAmtUSD;
            requestForm.BudgetApprovedAmtEuro = entity.BudgetApprovedAmtEuro;
            requestForm.BudgetSpentAmt = entity.BudgetSpentAmt;
            requestForm.BudgetSpentAmtUSD = entity.BudgetSpentAmtUSD;
            requestForm.BudgetSpentAmtEuro = entity.BudgetSpentAmtEuro;
            requestForm.Remarks = entity.Remarks;
            return requestForm;
        }

        internal static IEnumerable<RequestForm> ToModel(List<PaymentRequestForm> list)
        {
            if (list == null) return null;
            return list.Select(s => ToModel(s));
        }

        public RequestForm()
        {
            RequestQuestions = new List<RequestQuestion>();
        }
    }



    public class RequestFormDetails
    {
        public string DocumentType { get; internal set; }
        public string CountryName { get; internal set; }
        public int Month { get; set; }
        public string Year { get; set; }
        public string DepartmentName { get; internal set; }
        public string DepartmentsAccountName { get; internal set; }       
        public string Description { get; internal set; }
        public string ExpenseTypeName { get; internal set; }
        public string FrequencyTypeName { get; internal set; }
        public string PayeeName { get; internal set; }
        public string PayingEntityName { get; internal set; }
        public byte[] PayingEntityLogo { get; internal set; }
        public string PayingEntityCode { get; internal set; }
        public string PaymentMethodName { get; internal set; }
        public string Remarks { get; internal set; }
        public string Clarifications { get; set; }
        public CurrencyType CurrencyType { get; internal set; }
        public List<GoodsAndService> GoodsAndServices { get; internal set; }
        public List<PaymentRequestDocs> PaymentRequestDocuments { get; internal set; }
        public decimal SubTotal { get; internal set; }
        public string TaxType2Name { get; internal set; }
        public string TaxType3Name { get; internal set; }
        public decimal TaxAmount2 { get; internal set; }
        public decimal TaxAmount3 { get; internal set; }
        public decimal Total { get; internal set; }

        public List<RequestFormApprovalStatusDetails> Approvals { get; set; }
        public RequestFormOriginator Originator { get; internal set; }
        public decimal TaxAmount1 { get; internal set; }

        public RequestFormDetails()
        {
            GoodsAndServices = new List<GoodsAndService>();
        }

        public PayeeModel Payee { get; set; }
        public PayeeBankDetails PayeeBankDetails { get; set; }


        public string Status { get; set; }
        public decimal TaxAmount1USD { get; private set; }
        public decimal TaxAmount1Euro { get; private set; }
        public decimal TaxAmount2USD { get; private set; }
        public decimal TaxAmount2Euro { get; private set; }
        public decimal TaxAmount3USD { get; private set; }
        public decimal TaxAmount3Euro { get; private set; }
        public decimal TotalUSD { get; private set; }
        public decimal TotalEuro { get; private set; }
        public decimal SubTotalUSD { get; private set; }
        public decimal SubTotalEuro { get; private set; }
        public int PPRFNumber { get; internal set; }
        public DateTime DueDate { get; private set; }
        public DateTime PprfDate { get; private set; }
        public string PayingEntityLogoName { get; private set; }
        public string DocumentName { get; private set; }
        //public string DocumentPath { get; private set; }
        public string RequestId { get; private set; }


        public decimal USDExRate { get; private set; }
        public decimal EuroExRate { get; private set; }
        public string DocumentSavedName { get; private set; }

        internal string CreatedByName { get; private set; }
        internal string CreatedByEmail { get; private set; }

        public string BudgetPPRFNo { get; private set; }
        public DateTime? BudgetValidFrom { get; private set; }
        public DateTime? BudgetValidTo { get; private set; }
        public string BudgetApprovedAmtDesc { get; private set; }
        public string BudgetApprovedAmtUSDDesc { get; private set; }
        public string BudgetApprovedAmtEuroDesc { get; private set; }
        public string BudgetSpentAmtDesc { get; private set; }
        public string BudgetSpentAmtUSDDesc { get; private set; }
        public string BudgetSpentAmtEuroDesc { get; private set; }
        public decimal BudgetRemainingAmtUSD { get; private set; }
        public string BudgetRemainingAmtUSDDesc { get; private set; }
        public string BudgetOverSpentDesc { get; private set; }
        public string RestrictedPayeeOnlyFlag { get; private set; }
        public string BudgetValidityDesc { get; private set; }
        public string UrgentRemark { get; private set; }
        public string Note { get; private set; }
        
        internal static RequestFormDetails ToModel(Data.PaymentRequestForm requestForm)
        {
            byte[] image = null;
            if (requestForm == null) return null;
            if (!string.IsNullOrEmpty(requestForm.PayingEntity.LogoName))
            {
                try
                {
                    System.Drawing.Image img = System.Drawing.Image.FromFile(Path.Combine(Constants.PAYING_ENTITY_LOGO_BASE_PATH, requestForm.PayingEntity.LogoName));
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        image = ms.ToArray();
                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
            var details = new RequestFormDetails();

            details.RequestId = requestForm.Id.ToString();
            details.PprfDate = requestForm.PprfDate;
            details.DueDate = requestForm.DueDate;
            details.DocumentType = requestForm.DocumentType;
            details.DocumentName = requestForm.DocumentName;
            //details.DocumentPath = requestForm.DocumentPath;
            details.DocumentSavedName = requestForm.DocumentSavedName;
            details.PayingEntityName = requestForm.PayingEntity.Name;
            details.PayingEntityLogo = image;
            details.PayingEntityLogoName = string.IsNullOrEmpty(requestForm.PayingEntity.LogoName) ? null : requestForm.PayingEntity.LogoName;
            details.PayingEntityCode = requestForm.PayingEntity.Abbreviation;
            details.CountryName = requestForm.Country != null ? requestForm.Country.Name : string.Empty;
            details.DepartmentName = requestForm.Department.Name;
            details.DepartmentsAccountName = requestForm.DepartmentsAccount.Name ;
            details.Month = requestForm.Month;
            details.Year = new DateTime(requestForm.Year, 1, 31).ToString("yyyy");
            details.PPRFNumber = requestForm.AutoGeneratedSequence;
            details.FrequencyTypeName = requestForm.FrequencyType.Name;
            details.PaymentMethodName = requestForm.PaymentMethod.Name;
            details.ExpenseTypeName = requestForm.ExpenseType.Name;

            if (requestForm.DocumentType == "BDG" && requestForm.RestrictedPayeeOnly == false)
            {
                details.PayeeName = "-";
            }
            else {
                details.PayeeName = requestForm.PayeeBankAccountDetail.Payee.Name;
            }

            
            details.Description = requestForm.Description;
            details.Remarks = requestForm.Remarks;
            details.Clarifications = requestForm.Clarifications;
            details.Status = requestForm.Status;

            details.CurrencyType = new CurrencyType
            {
                Name = requestForm.Currency.Name,
                Code = requestForm.Currency.Code,
                USDValue = requestForm.Currency.USDValue,
                EuroValue = requestForm.Currency.EuroValue
            };
            details.USDExRate = requestForm.USDExRate;
            details.EuroExRate = requestForm.EuroExRate;

            details.GoodsAndServices = requestForm.GoodsAndServices.Select(g => new Models.GoodsAndService
            {
                Description = g.Description,
                Amount = g.Amount,
                Quantity = g.Quantity,
                TaxType = new Models.TaxType
                {
                    Name = g.TaxType == null ? string.Empty : g.TaxType.Name,
                    PercentageValue = g.TaxType == null ? 0m : g.TaxType.PercentageValue,
                    Id= g.TaxType == null ? string.Empty : g.TaxType.Id.ToString(),
                    Description= g.TaxType == null ? string.Empty : g.TaxType.Description
                },
                AmountUSD = g.AmountUSD,
                AmountEuro = g.AmountEuro
            }).ToList();

            details.PaymentRequestDocuments = requestForm.PaymentRequestDocuments.Select(d => new Models.PaymentRequestDocs
            {
                Id = d.Id.ToString(),
                DocumentName = d.DocumentName,
                DocumentSavedName = d.DocumentSavedName
            }).ToList();

            details.SubTotal = requestForm.GoodsAndServices.Sum(su => su.Amount);
            details.SubTotalUSD = requestForm.GoodsAndServices.Sum(su => su.AmountUSD);
            details.SubTotalEuro = requestForm.GoodsAndServices.Sum(su => su.AmountEuro);
            details.TaxType2Name = requestForm.Tax2Type == null ? string.Empty : requestForm.Tax2Type.Name;
            details.TaxType3Name = requestForm.Tax3Type == null ? string.Empty : requestForm.Tax3Type.Name;
            details.TaxAmount1 = requestForm.GoodsAndServices.Sum(su => su.TaxAmount);
            details.TaxAmount1USD = requestForm.GoodsAndServices.Sum(su => su.TaxAmountUSD);
            details.TaxAmount1Euro = requestForm.GoodsAndServices.Sum(su => su.TaxAmountEuro);
            details.TaxAmount2 = requestForm.Tax2Amount;
            details.TaxAmount2USD = requestForm.Tax2AmountUSD;
            details.TaxAmount2Euro = requestForm.Tax2AmountEuro;
            details.TaxAmount3 = requestForm.Tax3Amount;
            details.TaxAmount3USD = requestForm.Tax3AmountUSD;
            details.TaxAmount3Euro = requestForm.Tax3AmountEuro;
            details.Total = requestForm.GoodsAndServices.Sum(su => su.Amount + su.TaxAmount) + requestForm.Tax2Amount + requestForm.Tax3Amount;
            details.TotalUSD = requestForm.GoodsAndServices.Sum(su => su.AmountUSD + su.TaxAmountUSD) + requestForm.Tax2AmountUSD + requestForm.Tax3AmountUSD;
            details.TotalEuro = requestForm.GoodsAndServices.Sum(su => su.AmountEuro + su.TaxAmountEuro) + requestForm.Tax2AmountEuro + requestForm.Tax3AmountEuro;

            details.Originator = new RequestFormOriginator
            {
                OriginatorType = "Originator",
                OriginatorName = requestForm.Originator.Name,
                OriginatorEmail = requestForm.Originator.Email,
                OriginatorDepartmentName = requestForm.Originator.Department == null ? string.Empty : requestForm.Originator.Department.Name,
                OriginatorDesignation = requestForm.Originator.Designation,
                OriginatorStatus = "Started",
                Remarks = requestForm.Remarks == null ? string.Empty:"",
                ResponseDate = requestForm.CreatedOn
            };
            details.Approvals = requestForm.Approvers.OrderBy(o => o.SequenceNo).Select(su => new RequestFormApprovalStatusDetails
            {
                ApproverType = su.Approver.ApplicationGroup.Name,
                ApproverName = su.Approver.AppUser.Name,
                ApproverEmail = su.Approver.AppUser.Email,
                ApproverDepartmentName = su.Approver.AppUser.Department == null ? string.Empty : su.Approver.AppUser.Department.Name,
                ApproverDesignation = su.Approver.AppUser.Designation,
                ApprovalStatus = su.ApprovalStatus == Data.ApprovalStatus.PENDING ?
                                                    "Pending" : su.ApprovalStatus == Data.ApprovalStatus.APPROVED ?
                                                                                    "Approved" : "Rejected",
                Remarks = su.Remarks,
                ResponseDate = su.ResponseDate,
                ApproverToken = su.ApprovalToken,
                SequenceNumber = su.SequenceNo,
                Questions = su.Queries.Select(s => new QuestionAndAnswer
                {
                    Question = s.Question,
                    Answer = s.Answer,
                    AskedOn = s.AskedOn.ToString(Constants.DATEFORMAT),
                    AnsweredOn = s.AnsweredOn.HasValue ? s.AnsweredOn.Value.ToString(Constants.DATEFORMAT) : string.Empty
                }).ToList()
            }).ToList();


            if (requestForm.DocumentType == "BDG" && requestForm.RestrictedPayeeOnly == false)
            {
                details.Payee = new Models.PayeeModel
                {
                    Name = "-",
                    AddressLine1 = "-",
                    AddressLine2 = "-",
                    AddressLine3 = "-",
                    Fax = "-",
                    Phone = "-",
                    HotelName = "-",
                    CountryName = "-"
                };

                details.PayeeBankDetails = new PayeeBankDetails
                {
                    AccountName = "-",
                    AccountNumber = "-",
                    AccountType = "-",
                    BankAddress = "-",
                    BankName = "-",
                    IBAN = "-",
                    IFSC = "-",
                    Swift = "-"
                };
            }
            else
            {
                details.Payee = new Models.PayeeModel
                {
                    Name = requestForm.PayeeBankAccountDetail.Payee.Name,
                    AddressLine1 = requestForm.PayeeBankAccountDetail.Payee.AddressLine1,
                    AddressLine2 = requestForm.PayeeBankAccountDetail.Payee.AddressLine2,
                    AddressLine3 = requestForm.PayeeBankAccountDetail.Payee.AddressLine3,
                    Fax = requestForm.PayeeBankAccountDetail.Payee.Fax,
                    Phone = requestForm.PayeeBankAccountDetail.Payee.Phone,
                    HotelName = requestForm.PayeeBankAccountDetail.Payee.HotelName,
                    CountryName = requestForm.PayeeBankAccountDetail.Payee.Country != null ? requestForm.PayeeBankAccountDetail.Payee.Country.Name : string.Empty
                };

                details.PayeeBankDetails = new PayeeBankDetails
                {
                    AccountName = requestForm.PayeeBankAccountDetail.AccountName,
                    AccountNumber = requestForm.PayeeBankAccountDetail.AccountNumber,
                    AccountType = requestForm.PayeeBankAccountDetail.AccountType,
                    BankAddress = requestForm.PayeeBankAccountDetail.BankAddress,
                    BankName = requestForm.PayeeBankAccountDetail.BankName,
                    IBAN = requestForm.PayeeBankAccountDetail.IBAN,
                    IFSC = requestForm.PayeeBankAccountDetail.IFSC,
                    Swift = requestForm.PayeeBankAccountDetail.Swift
                };
            }

            details.IsClosed = requestForm.IsClosed;
            details.PprfNo = requestForm.PPRFNo;

            if (details.DocumentType == "PFB")
            {
                details.BudgetPPRFNo = requestForm.BudgetPPRFNo.PPRFNo != null ? requestForm.BudgetPPRFNo.PPRFNo : "-";
                details.BudgetApprovedAmtDesc = requestForm.BudgetPPRFNo.Currency.Code + " " + requestForm.BudgetApprovedAmt.ToString("N2");
                details.BudgetApprovedAmtUSDDesc = "USD " + requestForm.BudgetApprovedAmtUSD.ToString("N2");
                details.BudgetApprovedAmtEuroDesc = "EUR " + requestForm.BudgetApprovedAmtEuro.ToString("N2");
                details.BudgetSpentAmtUSDDesc = "USD " + requestForm.BudgetSpentAmtUSD.ToString("N2");
                details.BudgetSpentAmtEuroDesc = "EUR " + requestForm.BudgetSpentAmtEuro.ToString("N2");

                details.BudgetRemainingAmtUSD = requestForm.BudgetApprovedAmtUSD - requestForm.BudgetSpentAmtUSD - details.TotalUSD;
                details.BudgetRemainingAmtUSDDesc = "USD " + details.BudgetRemainingAmtUSD.ToString("N2");

                if (details.BudgetRemainingAmtUSD < 0)
                {
                    details.BudgetOverSpentDesc = "** Budget over spent **";
                }
                else
                {
                    details.BudgetOverSpentDesc = "";
                }
            }
            else if (details.DocumentType == "BDG")
            {
                details.BudgetValidFrom = requestForm.BudgetValidFrom;
                details.BudgetValidTo = requestForm.BudgetValidTo;
                details.RestrictedPayeeOnlyFlag = requestForm.RestrictedPayeeOnly? "Yes": "No";
                details.BudgetValidityDesc = requestForm.BudgetValidFrom.Value.ToString("dd/MMM/yyyy") + " - " +
                                             requestForm.BudgetValidTo.Value.ToString("dd/MMM/yyyy");
            }
            
            details.CreatedByName = requestForm.CreatedBy.Name;
            details.CreatedByEmail = requestForm.CreatedBy.Email;
            return details;
        }

        public bool IsClosed { get; set; }

        public string PprfNo { get; set; }
    }

    public class RequestFormOriginator
    {
        public string OriginatorType { get; set; }
        public string OriginatorName { get; set; }
        public string OriginatorDepartmentName { get; set; }
        public string OriginatorDesignation { get; set; }
        public string OriginatorStatus { get; set; }
        public DateTime ResponseDate { get; set; }
        public string Remarks { get; set; }
        public string OriginatorEmail { get; internal set; }
    }

    public class RequestFormApprovalStatusDetails
    {
        public string ApproverType { get; set; }
        public string ApproverName { get; set; }
        public string ApproverDepartmentName { get; set; }
        public string ApproverDesignation { get; set; }
        public string ApprovalStatus { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string Remarks { get; set; }
        public string ApproverToken { get; internal set; }
        public string ApproverEmail { get; internal set; }
        public int SequenceNumber { get; internal set; }
        public int QueryCount { get; internal set; }
        public string Note { get; internal set; }
        
        public bool HasPendingQuestions { get; internal set; }

        public List<QuestionAndAnswer> Questions { get; set; }

        public RequestFormApprovalStatusDetails()
        {
            Questions = new List<QuestionAndAnswer>();
        }
    }


    public class OriginatorFormsViewModel
    {
        public Paging Pager { get; set; }
        public List<RequestForm> RequestForms { get; set; }

        public OriginatorFormsViewModel()
        {
            RequestForms = new List<RequestForm>();
        }
    }

    


    public class ApprovedRequestsHomeViewModel
    {
        public ClosedRequestsViewModel ClosedRequestsViewModel { get; set; }
    }
    public class ApprovedFormsViewModel
    {
        public Paging Pager { get; set; }
        public List<RequestForm> RequestForms { get; set; }

        public ApprovedFormsViewModel()
        {
            RequestForms = new List<RequestForm>();
        }
    }


    public class ListItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class ClosedRequestsSearchCriteria
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string PayingEntityId { get; set; }
        public string Status { get; set; }

        public List<ListItem> Statuses { get; set; }

        //public List<PayingEntity> PayingEntities { get; set; }

        public ClosedRequestsSearchCriteria()
        {
            Statuses = new List<ListItem>();
            //PayingEntities = new List<PayingEntity>();
        }
    }
    public class ClosedRequestsViewModel
    {
        public Sorting Sort { get; set; }
        public Paging Pager { get; set; }
        public ClosedRequestsSearchCriteria Filters { get; set; }
        public List<RequestForm> RequestForms { get; set; }

        public ClosedRequestsViewModel()
        {
            RequestForms = new List<RequestForm>();
        }
    }

    public class RequestFormsViewModel
    {
        public Sorting Sort { get; set; }
        public Paging Pager { get; set; }

        public ClosedRequestsSearchCriteria Filters { get; set; }
        public List<RequestForm> RequestForms { get; set; }

        public RequestFormsViewModel()
        {
            RequestForms = new List<RequestForm>();
        }
    }

    public class ApproveRejectRequestModel
    {
        public string token { get; set; }
        public string v { get; set; }
        public string Description { get; set; }
        public IEnumerable<WebApp.DAL.Data.RejectTypes> rejectsTypes { get; set; }
    }


    public class GiveClarificationModel
    {
        [Required]
        public string RequestId { get; set; }
        public string Clarifications { get; set; }
    }


    
}
