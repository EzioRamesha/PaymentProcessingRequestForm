using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Helpers;

namespace WebApp.DAL.Data
{
    public class ApproverRequestQuestion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        public Guid ApprovalId { get; set; }
        [ForeignKey("ApprovalId")]
        public RequestApprover RequestApprover { get; set; }


        public string Question { get; set; }
        public string Answer { get; set; }

        public DateTime AskedOn { get; set; }
        public DateTime? AnsweredOn { get; set; }


        public ApproverRequestQuestion()
        {
            AskedOn = GeneralHelper.CurrentDate();
            AnsweredOn = null;
        }

        //public ApproverRequestQuestion(string question) : this()
        //{
        //    this.Question = question;
        //}
    }
}
