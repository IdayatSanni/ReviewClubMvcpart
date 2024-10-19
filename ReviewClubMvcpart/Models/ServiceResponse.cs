namespace ReviewClubMvcpart.Models
{
    public class ServiceResponse
    {
        public enum ServiceStatus { NotFound, Created, Updated, Deleted, Error, Success }

        public ServiceStatus Status { get; set; }

        public int CreatedId { get; set; }

        // ServiceResponse package allows for more information, such as logic / validation errors
        public List<string> Messages { get; set; } = new List<string>();
    }
}
