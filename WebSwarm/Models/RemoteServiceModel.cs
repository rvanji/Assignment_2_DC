namespace WebSwarm.Models
{
    public class RemoteServiceModel
    {
        public int? RemoteServiceID { get; set; }
        public string? IPAddress { get; set; }
        public int? Port { get; set; }
        public int NoOfJobs { get; set; }
        public bool? IsAllocated { get; set; }
    }

    public class RemoteServicesModel
    {
        public ClientModel? Client { get; set; }
        public string? Token { get; set; }
        public List<RemoteServiceModel> RemoteServices { get; set; } = new List<RemoteServiceModel>();
        public string? ErrorMessage { get; set; }
    }
}
