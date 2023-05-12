using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMWellPlatformConsole.Model;

public class Well
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int PlatformId { get; set; } //foreign key
    public string UniqueName { get; set; } = ""; 
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Platform Plaftform { get; set; }
    
}