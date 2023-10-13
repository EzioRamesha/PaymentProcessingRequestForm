using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.DAL.Data
{
    public class PaymentRequestDocuments
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string DocumentName { get; set; }
        public string DocumentSavedName { get; set; }
        //public string DocumentPath { get; set; }
        public virtual Guid PaymentRequestFormId { get; set; }
        [ForeignKey("PaymentRequestFormId")]
        public virtual PaymentRequestForm PaymentRequestForm { get; set; }
    }
}