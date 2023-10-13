using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApp.DAL.BAL;

namespace WebApp.Controllers
{
    [RoutePrefix("api/request")]
    public class WebRequestController : ApiController
    {
        private readonly RequestFormBAL _requestFormBAL = new RequestFormBAL();

        //[Route("Approve")]
        //[HttpGet]
        //public HttpResponseMessage Approve([FromUri]string token, [FromUri]string v)
        //{
        //    var result = _requestFormBAL.Approve(token, v);
        //    return Request.CreateResponse(HttpStatusCode.OK, new {
        //        value = result
        //    });
        //}


        //[Route("Reject")]
        //[HttpGet]
        //public HttpResponseMessage Reject([FromUri]string token, [FromUri]string v)
        //{
        //    var returnValue = _requestFormBAL.Reject(token, v);
        //    return Request.CreateResponse(HttpStatusCode.OK, new
        //    {
        //        value = true
        //    });
        //}

        [HttpGet]
        public HttpResponseMessage Test([FromUri] string s, [FromUri] string name)
        {
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}
