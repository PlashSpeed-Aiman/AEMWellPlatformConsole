using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AEMWellPlatformConsole.Model;

public class Platform
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public int Id { get; set; }

    public string UniqueName { get; set; } = "";
    public double Latitude { get; set; } = 0.0;
    public double Longitude { get; set; } = 0.0;
    public DateTime? CreatedAt { get; set; } 
    public DateTime? UpdatedAt { get; set; } 
    [JsonPropertyName("well")]
    public List<Well> Wells { get; set; }

}