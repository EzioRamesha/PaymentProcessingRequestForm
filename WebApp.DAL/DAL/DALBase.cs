using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;

namespace WebApp.DAL.DAL
{
    internal class DALBase : IDisposable
    {
        public ApplicationDbContext _dbContext;

        public DALBase()
        {
            _dbContext = new ApplicationDbContext();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
        //public static readonly DatabaseContext _dbContext = new DatabaseContext();
    }
}
