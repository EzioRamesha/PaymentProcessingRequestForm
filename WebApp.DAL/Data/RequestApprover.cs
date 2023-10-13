using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Data
{
    public enum ApprovalStatus
    {
        PENDING,
        APPROVED,
        REJECTED
    }
    public class RequestApprover
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        public Guid ApproverId { get; set; }
        [ForeignKey("ApproverId")]
        public virtual ApplicationUserGroup Approver { get; set; }



        public Guid PaymentRequestFormId { get; set; }
        [ForeignKey("PaymentRequestFormId")]
        public virtual PaymentRequestForm PaymentRequestForm { get; set; }

        public ApprovalStatus ApprovalStatus { get; set; }


        public virtual List<ApproverRequestQuestion> Queries { get; set; }
        public bool IsQuestionAsked { get; set; }

        public string ApprovalToken { get; set; }


        public int SequenceNo { get; set; }

        public string Remarks { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string Reason { get; internal set; }

        public RequestApprover()
        {
            Queries = new List<ApproverRequestQuestion>();
            IsQuestionAsked = false;
        }
    }
}
