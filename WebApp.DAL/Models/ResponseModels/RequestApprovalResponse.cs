using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Models.ResponseModels
{
    public class RequestApprovalResponse
    {
        public string DocumentType { get; set; }
        public string DocumentNo { get; set; }
    }

    public class RequestRejectResponse
    {
        public string DocumentType { get; set; }
        public string DocumentNo { get; set; }
        public string OriginatorName { get; set; }
    }

    public class RequestClarificationResponse
    {
        public string DocumentType { get; set; }
        public string DocumentNo { get; set; }
        public string OriginatorName { get; set; }
    }
}
