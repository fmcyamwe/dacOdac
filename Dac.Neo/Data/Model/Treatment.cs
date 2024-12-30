using System.ComponentModel.DataAnnotations;

namespace Dac.Neo.Model; 

public class Treatment(string name, string by)
{
    [Required]
    public string Name { get; set; } = name;
    
    [Required]
    public string PrescribedBy { get; set;} = by; //doctor's Id 

    public string? Details { get; set;}
    
    public DateTime StartDate { get; set;}

    public DateTime? EndDate { get; set;}
}