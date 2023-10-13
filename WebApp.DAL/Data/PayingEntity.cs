using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Data
{
    public class PayingEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
        //public byte[] Logo { get; set; }
        public string LogoName { get; set; }
        public bool IsEnabled { get; set; }



        public Guid? PreviousDetailsId { get; set; }
        [ForeignKey("PreviousDetailsId")]
        public PayingEntity PreviousDetails { get; set; }
        public virtual List<PayingEntity> PreviousEdits { get; set; }


        public virtual List<EntityAmountRange> RangeConfig { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }



        public PayingEntity()
        {
            IsEnabled = true;
        }

        public PayingEntity(string name): this()
        {
            Name = name;
        }
        public PayingEntity(string Name, string Abbreviation) : this()
        {
            this.Name = Name;
            this.Abbreviation = Abbreviation;
        }
        public PayingEntity(string Name, string Abbreviation, bool IsEnabled) : this(Name, Abbreviation)
        {
            this.IsEnabled = IsEnabled;
        }
    }
}
