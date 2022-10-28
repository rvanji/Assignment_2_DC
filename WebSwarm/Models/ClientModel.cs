namespace WebSwarm.Models
{
    public class ClientModel
    {
        public int? ClientID { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsOnline { get; set; }
        public int StatusID { get; set; }

        public string? Token { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
