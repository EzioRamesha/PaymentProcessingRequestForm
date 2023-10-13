using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.App_Start.Attributes;
using WebApp.DAL.BAL;
using WebApp.DAL.Models;
using WebApp.DAL.Helpers;
using WebApp.DAL.Models.ResponseModels;

namespace WebApp.Controllers
{
    [CustomAuthorize("Admin,Operator")]
    public class HomeController : BaseController
    {
        private static readonly RequestFormBAL _requestFormBAL = new RequestFormBAL();
        private static readonly UserBAL _userBAL = new UserBAL();
        private static readonly PayingEntityBAL _payingEntityBAL = new PayingEntityBAL();
        private static readonly DepartmentBAL _departmentBAL = new DepartmentBAL();
        private static readonly DepartmentsAccountBAL _marketingDepartmentBAL = new DepartmentsAccountBAL();
        private static readonly PayeeBAL _payeeBAL = new PayeeBAL();

        public ActionResult Index()
        {
            return View();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult FormsPendingApproval(RequestFormsViewModel model)
        {
            var user = _userBAL.FindByEmail(User.Identity.Name);
            if (user != null)
            {
                model = _requestFormBAL.RequestFormsForApprover(user.Email, model);
            }
            return Content(JsonConvert.SerializeObject(model));
        }

        [MyValidateAntiForgeryToken]
        public ContentResult FormsForOriginator(OriginatorFormsViewModel model)
        {
            var user = _userBAL.FindByEmail(User.Identity.Name);
            //var user = _userBAL.FindByEmail("sameer.anand2711@gmail.com");
            if (user != null)
            {
                model = _requestFormBAL.RequestFormsForOriginatorOrCreator(user.Email, model);
                //model = _requestFormBAL.RequestFormsForOriginator(user.Email, model);
            }
            return Content(JsonConvert.SerializeObject(model));
        }

        [Permission("Close PO/PPRF")]
        public ActionResult ApprovedRequests()
        {
            return View();
        }
        [Permission("Close PO/PPRF")]
        public ActionResult FilterCriteria()
        {
            var model = new
            {
                Statuses = new List<ListItem>() {
                    new ListItem { Name = "All", Value = "All" },
                    new ListItem { Name = "Pending", Value = "Pending" },
                    new ListItem { Name = "Approved", Value = "Approved" },
                    new ListItem { Name = "Rejected", Value = "Rejected" },
                    new ListItem { Name = "Closed", Value = "Closed" },
                    new ListItem { Name = "Cancelled", Value = "Cancelled" }
                },
                PayingEntities = _payingEntityBAL.GetActive(),
                Departments = _departmentBAL.GetActive(),
                Payees = _payeeBAL.GetActive()
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [MyValidateAntiForgeryToken]
        [Permission("Close PO/PPRF")]
        public ContentResult ApprovedFormsForClosure(ApprovedFormsViewModel model)
        {
            var user = _userBAL.FindByEmail(User.Identity.Name);
            if (user != null)
            {
                model = _requestFormBAL.GetApprovedFormsPendingClosure(model);
            }
            return Content(JsonConvert.SerializeObject(model));
        }

        [MyValidateAntiForgeryToken]
        [Permission("Close PO/PPRF")]
        public ContentResult GetClosedForms(ClosedRequestsViewModel model)
        {
            if (model != null)
            {
                //model = _requestFormBAL.GetClosedRequests(model);
                model = _requestFormBAL.GetClosedOrRejectedRequests(model);
            }
            return Content(JsonConvert.SerializeObject(model));
        }

        [MyValidateAntiForgeryToken]
        [Permission("Close PO/PPRF")]
        public ContentResult GetAllForms(RequestFormsViewModel model)
        {
            ResponseObject<RequestFormsViewModel> response = null;
            if (model != null)
            {
                //model = _requestFormBAL.GetClosedRequests(model);
                response = _requestFormBAL.GetAllRequests(model);
            }
            if (response == null)
            {
                response = new ResponseObject<RequestFormsViewModel>
                {
                    ResponseType = "error",
                    Message = "Something went wrong. Please contact the administrator."
                };
            }
            return Content(JsonConvert.SerializeObject(response));
        }


        [Permission("Close PO/PPRF")]
        public ActionResult ExportClosedForms(RequestFormsViewModel model)
        {
            string fileName = "Requests-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            //var closedForms = _requestFormBAL.GetClosedRequests(model);
            //var forms = _requestFormBAL.GetClosedOrRejectedRequests(model);
            var forms = _requestFormBAL.GetAllRequests(model);
            DataTable dt = GetDataTableForForms(forms.Data.RequestForms);
            var excelFileStream = CreateExcel(dt, "Closed Forms");
            var bytes = excelFileStream.ToArray();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }



        private DataTable GetDataTableForForms(List<RequestForm> closedForms)
        {
            var dt = new DataTable();
            dt.Columns.Add("#");
            dt.Columns.Add("Document Type");
            dt.Columns.Add("PPRF No");
            dt.Columns.Add("PPRF Date");
            dt.Columns.Add("Payment Due");
            dt.Columns.Add("Document Status");
            dt.Columns.Add("Paying Entity");
            dt.Columns.Add("Department");
            dt.Columns.Add("Originator");
            dt.Columns.Add("Payee");
            dt.Columns.Add("Description");
            dt.Columns.Add("Total Inc. Tax (Local)");
            dt.Columns.Add("Total Inc. Tax (USD)");
            dt.Columns.Add("Total Tax (Local)");
            dt.Columns.Add("Total Tax (USD)");

            foreach (var item in closedForms.Select((obj, idx) => new { obj, idx }))
            {
                var requestNumber = item.obj.DocumentType + "/"
                                    + item.obj.PayingEntityCode + "/"
                                    + string.Format("{0:00}", item.obj.Month) + (new DateTime(Convert.ToInt32(item.obj.Year), 01, 01)).ToString("yy") + "/"
                                    + string.Format("{0:000000}", item.obj.Number);
                var dr = dt.NewRow();
                dr["#"] = item.idx + 1;
                dr["Document Type"] = item.obj.DocumentType;
                dr["PPRF No"] = requestNumber;
                dr["PPRF Date"] = item.obj.PprfDate.ToString("dd/MMM/yyyy");
                dr["Payment Due"] = item.obj.DueDate.ToString("dd/MMM/yyyy");
                dr["Document Status"] = item.obj.Status;
                dr["Paying Entity"] = item.obj.PayingEntityName;
                dr["Department"] = item.obj.Originator.Department.Name;
                dr["Originator"] = item.obj.OriginatorName;
                dr["Payee"] = item.obj.PayeeName;
                dr["Description"] = item.obj.Description;
                dr["Total Inc. Tax (Local)"] = item.obj.CurrencyCode + " " + string.Format("{0:0.00}", item.obj.TotalValueIncTax);
                dr["Total Inc. Tax (USD)"] = "USD " + string.Format("{0:0.00}", item.obj.TotalValueIncTaxInUSD);
                dr["Total Tax (Local)"] = item.obj.CurrencyCode + " " + string.Format("{0:0.00}", item.obj.TotalTax);
                dr["Total Tax (USD)"] = "USD " + string.Format("{0:0.00}", item.obj.TotalTaxInUSD);

                dt.Rows.Add(dr);
            }
            return dt;
        }
        private MemoryStream CreateExcel(DataTable dt, string sheetName)
        {
            IWorkbook workbook = new XSSFWorkbook();
            IFont boldFont = workbook.CreateFont();
            boldFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            ISheet sheet1 = workbook.CreateSheet(sheetName);

            IRow row1 = sheet1.CreateRow(0);

            for (int j = 0; j < dt.Columns.Count; j++)
            {

                ICell cell = row1.CreateCell(j);
                string columnName = dt.Columns[j].ToString();
                cell.SetCellValue(columnName);
                cell.CellStyle = workbook.CreateCellStyle();
                cell.CellStyle.SetFont(boldFont);
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row.CreateCell(j);
                    string columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(dt.Rows[i][columnName].ToString());
                }
            }
            var exportData = new MemoryStream();
            workbook.Write(exportData);
            return exportData;
        }
    }
}