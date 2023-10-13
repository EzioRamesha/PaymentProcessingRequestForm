using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.App_Start.Attributes;
using WebApp.DAL.BAL;
using WebApp.DAL.Models;
using WebApp.DAL.Models.ResponseModels;

namespace WebApp.Controllers
{
    [CustomAuthorize("Viewer")]
    public class ViewerController : Controller
    {

        private static readonly RequestFormBAL _requestFormBAL = new RequestFormBAL();


        // GET: Viewer
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult ListAllRequests(RequestFormsViewModel model)
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
            return Content(JsonConvert.SerializeObject(model));
        }

        public ContentResult GetRequestDetails(string Id)
        {
            //RequestFormDetails request = _requestFormBAL.GetRequestDetailsById(Id);
            ResponseObject<RequestFormDetails> request = null;
            return Content(JsonConvert.SerializeObject(request));
        }
    }
}