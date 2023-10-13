using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Models
{
    public class Sorting
    {
        public string SortDirection { get; set; }
        public string SortParameter { get; set; }
    }
    public class Paging
    {
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }

        public Paging()
        {
            ItemsPerPage = 20;
            CurrentPage = 1;
        }
    }
}
