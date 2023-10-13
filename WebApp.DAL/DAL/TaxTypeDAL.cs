using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;

namespace WebApp.DAL.DAL
{
    internal class TaxTypeDAL : DALBase
    {
        public IQueryable<TaxType> List()
        {
            return _dbContext.TaxTypes;
        }
    }
}
