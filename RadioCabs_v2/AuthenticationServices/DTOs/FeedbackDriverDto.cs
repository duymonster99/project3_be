using System.ComponentModel.DataAnnotations;

namespace AuthenticationServices.DTOs
{
    public class FeedbackDriverDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Description { get; set; }
        public decimal? Rating { get; set; }
        public int? CompanyId { get; set; }
        public int? DriverId { get; set; }
    }
}
