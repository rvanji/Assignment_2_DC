using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class JobModel
    {
        public int? JobID { get; set; }
        public int? RemoteServiceID { get; set; }
        public string PythonCode { get; set; }
        public string Result { get; set; }
        public int StatusID { get; set; }
        public int? CreatedBy { get; set; }

        public string Token { get; set; }
        public ResponseModel Response { get; set; }
    }

    public class JobsModel
    {
        public List<JobModel> JobsList { get; set; }

        public ResponseModel Response { get; set; }
    }
}
