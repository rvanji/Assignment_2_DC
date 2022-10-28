using System.Collections.Generic;

namespace Common.Models
{
    public class RemoteServiceModel
    {
        public int? RemoteServiceID { get; set; }
        public string IPAddress { get; set; }
        public int? Port { get; set; }
        public int NoOfJobs { get; set; }
        public bool? IsAllocated { get; set; }
        public string ServiceStatus { get; set; }

        public ClientModel Client { get; set; }
        public string Token { get; set; }
        public ResponseModel Response { get; set; }
    }

    public class RemoteServicesModel
    {
        public List<RemoteServiceModel> RemoteServices { get; set; }

        public ResponseModel Response { get; set; }
    }
}