using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Models
{
    public class NewEntityAmountRange
    {
        public List<PayingEntity> PayingEntities { get; set; }
    }

    #region Add
    public class AddRange
    {
        public string PayingEntityId { get; set; }
        [Required]
        public decimal AmountFrom { get; set; }
        [Required]
        public decimal AmountTo { get; set; }
    }
    public class AddEmail
    {
        public string rangeId { get; set; }
        [MinLength(1)]
        public string[] Emails { get; set; }
    }
    //public class AddRangeEmails
    //{
    //    [Required]
    //    public decimal AmountFrom { get; set; }
    //    [Required]
    //    public decimal AmountTo { get; set; }
    //    public List<string> EmailAddresses { get; set; }
    //}
    //public class AddEntityAmountRange
    //{
    //    public string PayingEntityId { get; set; }

    //    public List<AddRangeEmails> RangeConfig { get; set; }
    //}
    #endregion

    #region Edit
    public class EditRangeEmails
    {
        [Required]
        public decimal AmountFrom { get; set; }
        [Required]
        public decimal AmountTo { get; set; }
        public List<string> EmailAddresses { get; set; }
    }
    public class EditEntityAmountRange
    {
        [Required]
        public string Id { get; set; }
        public string PayingEntityId { get; set; }

        public List<EditRangeEmails> RangeConfig { get; set; }
    }
    #endregion



    #region View
    public class RangeEmails
    {
        public decimal AmountFrom { get; set; }
        public decimal AmountTo { get; set; }
        public List<string> EmailAddresses { get; set; }
        public string Id { get; internal set; }
    }
    public class ViewEntityAmountRange
    {
        public PayingEntity PayingEntity { get; set; }
        public List<RangeEmails> AmountRangeConfig { get; set; }
    }





    public class DeleteEmailFromRange
    {
        public string RangeId { get; set; }
        public string Email { get; set; }
    }
    #endregion
}
