using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Models.ResponseModels
{
    public class QuestionAnswer
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public string ApproverName { get; set; }
        public DateTime AskedOn { get; set; }
        public string AnsweredBy { get; set; }
        public DateTime? AnsweredOn { get; set; }
    }
}
