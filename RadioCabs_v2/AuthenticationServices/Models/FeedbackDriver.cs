using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AuthenticationServices.Models
{
    public class FeedbackDriver
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? Rating { get; set; }
        public int? CompanyId { get; set; }
        public int? DriverId { get; set; }
        [JsonIgnore]
        public virtual Driver? Driver { get; set; }
    }
}
