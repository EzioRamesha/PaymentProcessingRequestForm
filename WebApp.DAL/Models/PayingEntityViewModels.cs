using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebApp.DAL.Models
{
    public class AddPayingEntityViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
        public HttpPostedFileBase Logo { get; set; }
    }

    public class UpdatePayingEntityViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
        public HttpPostedFileBase Logo { get; set; }
    }

    public class PayingEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class ManagePayingEntitiesViewModel
    {
        public Paging Pager { get; set; }
        public List<PayingEntity> PayingEntities { get; set; }
        public ManagePayingEntitiesViewModel()
        {
            PayingEntities = new List<PayingEntity>();
        }
    }
}
