using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Models.ResponseModels
{
    public class FileDownloadModel
    {
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
    }
}
