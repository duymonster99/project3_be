using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace CompanyServices.Models;

public class CompanyLocationService
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? CompanyId { get; set; }

    public string? CityService { get; set; }
    [JsonIgnore]
    public virtual Company? Company { get; set; }
}