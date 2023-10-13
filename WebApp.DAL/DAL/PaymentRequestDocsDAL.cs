using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;
using WebApp.DAL.Helpers;

namespace WebApp.DAL.DAL
{
    internal class PaymentRequestDocsDAL : DALBase
    {
        public IQueryable<PaymentRequestDocuments> List()
        {
            return _dbContext.PaymentRequestDocuments;
        }
    }
}