using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace CompanyServices.Models
{
    public class AdvertisementImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        
        public string? ImageUrl { get; set; } // URL or path to the image
        public string? Description { get; set; } // Optional description
        
        [JsonIgnore]
        public virtual Company? Company { get; set; }
    }
}